using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader
{
	public class CsvReader : IDisposable
	{
		private TextReader TextReader { get; }

		public char EscapeChar { get; }

		public char DelimiterChar { get; }

		public LineEnding LineEnding { get; private set; }

		public bool CanEscape { get; }

		public bool PermitEmptyLineAtEnd { get; }

		private readonly ReadOnlyMemory<char> EscapeCharArray;

		private Memory<char> SearchArray;

		private MemorySequence<char> CharMemorySequence;

		private int NewCurrentOffset;

		private MemorySequenceSegment<char>? CurrentlyLoadingSegment;

#if NETSTANDARD2_1_OR_GREATER
		ValueTask<int> LoadingTask;
#else
		Task<int>? LoadingTask;
#endif

		private readonly List<MemorySequencePosition<char>> SkipPositions;

		private int BufferSizeInChars { get; }

		public long Position { get => this.CharMemorySequence.CurrentPosition.AbsolutePosition + NewCurrentOffset; }

		private readonly bool LeaveOpen;

		private char[]? TempCellArray;

		private bool NextNodeIsCell;

		public CsvReader(TextReader textReader)
			: this(textReader: textReader, options: new CsvReaderOptions())
		{ }

		public CsvReader(TextReader textReader, CsvReaderOptions options)
		{
			const int minBufferSize = 1;

			if(options==null)
				throw new ArgumentNullException(nameof(options));

			if(options.BufferSizeInChars<minBufferSize)
				throw new ArgumentOutOfRangeException($"{nameof(options)}.{nameof(options.BufferSizeInChars)}", options.BufferSizeInChars, $"{nameof(options)}.{nameof(options.BufferSizeInChars)} cannot be less than {minBufferSize}");

			this.TextReader = textReader??throw new ArgumentNullException(nameof(textReader));
			this.CanEscape = options.CanEscape;
			this.PermitEmptyLineAtEnd = options.PermitEmptyLineAtEnd;
			this.EscapeChar = options.EscapeChar;
			this.DelimiterChar = options.DelimiterChar;
			this.LineEnding = options.LineEnding;
			this.BufferSizeInChars = options.BufferSizeInChars;
			this.LeaveOpen = options.LeaveOpen;

			NextNodeIsCell = true;

			//Create data for SearchArray
			var searchArray = new char[2+(CanEscape ? 1 : 0)+(LineEnding==LineEnding.Auto ? 1 : 0)];
			int i = 0;
			if(CanEscape)
			{
				searchArray[i++] = EscapeChar;
				EscapeCharArray = new ReadOnlyMemory<char>(searchArray, 0, 1);
			}
			SearchArray = new Memory<char>(searchArray, i, searchArray.Length-i);
			searchArray[i++] = DelimiterChar;
			switch(LineEnding)
			{
				case LineEnding.Auto:
					searchArray[i++] = '\r';
					searchArray[i++] = '\n';
					break;
				case LineEnding.LF:
					searchArray[i++] = '\n';
					break;
				case LineEnding.CRLF:
				case LineEnding.CR:
					searchArray[i++] = '\r';
					break;
				default:
					throw new ArgumentException("Wrong line ending value", $"{nameof(options)}.{nameof(options.LineEnding)}");
			}

			//Add first segment to CharMemorySequence
			this.CharMemorySequence.AddNewSegment(minimumLength: this.BufferSizeInChars);

			this.SkipPositions = new List<MemorySequencePosition<char>>();
		}

		/// <summary>
		/// Method reads next csv node as <see cref="StringNode"/>
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <exception cref="System.Runtime.Serialization.SerializationException">Broken csv detected. End of stream inside escaped cell or data after end of escaped cell</exception>
		/// <returns>Next csv node read</returns>
		public async ValueTask<StringNode> ReadNextNodeAsStringAsync(CancellationToken cancellationToken = default)
		{
			var memorySequenceNode = await ReadNextNodeAsMemorySequenceAsync(cancellationToken)
				.ConfigureAwait(false);

			var data = MemorySequenceToString(memorySequenceNode.MemorySequence);

			return new StringNode(data, memorySequenceNode.NodeType);
		}

		/// <summary>
		/// Method converts <paramref name="memorySequenceSpan"/> to <see cref="string"/>
		/// </summary>
		/// <param name="memorySequenceSpan">Memory sequence span to convert</param>
		/// <returns><paramref name="memorySequenceSpan"/> converted to <see cref="string"/></returns>
		public string MemorySequenceToString(in MemorySequenceSpan memorySequenceSpan)
		{
			string str;

			var charsCount = memorySequenceSpan.CharsCount;
			if(0<charsCount)
			{
#if NETSTANDARD2_1_OR_GREATER
				str = string.Create(charsCount, memorySequenceSpan, (destination, msn) => msn.CopyDataTo(destination));
#else
				var memory = MemorySequenceAsMemory(memorySequenceSpan);
				str = memory.ToString();
#endif
			}
			else
				str = string.Empty;

			return str;
		}

		/// <summary>
		/// Method reads next csv node as <see cref="MemoryNode"/>.
		/// Returned <see cref="MemoryNode"/> is valid only till next ReadNextNodeAsXxx call.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <exception cref="System.Runtime.Serialization.SerializationException">Broken csv detected. End of stream inside escaped cell or data after end of escaped cell</exception>
		/// <returns>Next csv node read</returns>
		public async ValueTask<MemoryNode> ReadNextNodeAsMemoryAsync(CancellationToken cancellationToken = default)
		{
			var memorySequenceNode = await ReadNextNodeAsMemorySequenceAsync(cancellationToken)
				.ConfigureAwait(false);

			var memory = MemorySequenceAsMemory(memorySequenceNode.MemorySequence);

			return new MemoryNode(memory, memorySequenceNode.NodeType);
		}

		/// <summary>
		/// Method process <paramref name="memorySequenceSpan"/> to continous memory region and returns <see cref="ReadOnlyMemory{char}"/>.
		/// Returned <see cref="ReadOnlyMemory{char}"/> is valid only till next ReadNextNodeAsXxx call.
		/// </summary>
		/// <param name="memorySequenceSpan">Memory sequence span to process</param>
		/// <returns>Continous memory region of chars</returns>
		public ReadOnlyMemory<char> MemorySequenceAsMemory(in MemorySequenceSpan memorySequenceSpan)
		{
			//Check is it single segment
			if(object.ReferenceEquals(memorySequenceSpan.StartPosition.SequenceSegment, memorySequenceSpan.EndPosition.SequenceSegment))
			{
				//Single segment, only sliding is needed
				if(0<memorySequenceSpan.SkipCharPositions.Count)
				{
					int i, startHole, endHole;
					endHole = memorySequenceSpan.SkipCharPositions[0].PositionInSegment;
					for(i = 1; i<memorySequenceSpan.SkipCharPositions.Count; i++)
					{
						startHole = endHole+1;
						endHole = memorySequenceSpan.SkipCharPositions[i].PositionInSegment;
						Array.Copy(memorySequenceSpan.StartPosition.InternalSequenceSegment.Array, startHole, memorySequenceSpan.StartPosition.InternalSequenceSegment.Array, startHole-i, endHole-startHole);
					}
					startHole = endHole+1;
					endHole = memorySequenceSpan.EndPosition.PositionInSegment;
					Array.Copy(memorySequenceSpan.StartPosition.InternalSequenceSegment.Array, startHole, memorySequenceSpan.StartPosition.InternalSequenceSegment.Array, startHole-i, endHole-startHole);
				}
				return memorySequenceSpan.StartPosition.SequenceSegment.Memory.Slice(memorySequenceSpan.StartPosition.PositionInSegment, memorySequenceSpan.EndPosition.PositionInSegment-memorySequenceSpan.StartPosition.PositionInSegment-memorySequenceSpan.SkipCharPositions.Count);
			}

			//Provide memory for cell data
			var destination = ProvideMemory(memorySequenceSpan.StartPosition.InternalSequenceSegment, memorySequenceSpan.CharsCount);

			//Copy cell data to destination continous memory region
			var written = memorySequenceSpan.CopyDataTo(destination.Span);

			return destination;
		}

		private Memory<char> ProvideMemory(MemorySequenceSegment<char> sequenceSegment, int requiredSize)
		{
			char[] destination;

			if(requiredSize<=sequenceSegment.Array.Length)//Sequence segment is big enough to fit cell data
				destination = sequenceSegment.Array;
			else
			{
				if(TempCellArray==null)
					TempCellArray = ArrayPool<char>.Shared.Rent(requiredSize);
				else if(TempCellArray.Length<requiredSize)
				{
					ArrayPool<char>.Shared.Return(TempCellArray, true);
					TempCellArray = ArrayPool<char>.Shared.Rent(requiredSize);
				}
				destination = TempCellArray;
			}

			return new Memory<char>(destination, 0, requiredSize);
		}

		/// <summary>
		/// Method reads next csv node as memory sequence.
		/// Returned <see cref="MemorySequenceNode"/> is valid only till next ReadNextNodeAsXxx call.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <exception cref="System.Runtime.Serialization.SerializationException">Broken csv detected. End of stream inside escaped cell or data after end of escaped cell</exception>
		/// <returns>Next csv node read</returns>
		public async ValueTask<MemorySequenceNode> ReadNextNodeAsMemorySequenceAsync(CancellationToken cancellationToken = default)
		{
			CharMemorySequence.MoveForward(CharMemorySequence.CurrentPosition, NewCurrentOffset);
			NewCurrentOffset = 0;

			var found = await GetCharAsync(CharMemorySequence.CurrentPosition, 0, cancellationToken)
				.ConfigureAwait(false);

			//Check end of stream
			if(found.EndOfStream)
			{
				if(NextNodeIsCell && 0<found.FoundPosition.AbsolutePosition)//If file is empty it should return only end of stream
				{
					NextNodeIsCell = false;
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
				}
				return new MemorySequenceNode(found.FoundPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.EndOfStream);
			}

			//Check delimiter shortcut
			if(found.Character==DelimiterChar)
			{
				NewCurrentOffset = 1;
				NextNodeIsCell = true;
				return new MemorySequenceNode(found.FoundPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
			}

			//Check start of escaped cell
			if(found.Character==EscapeChar && CanEscape)
			{
				var currentPosition = CharMemorySequence.CurrentPosition;
				ReadCharResult readResult;

				SkipPositions.Clear();

				while(true)
				{
					readResult = await FindCharAsync(currentPosition, 1, EscapeCharArray, cancellationToken)
						.ConfigureAwait(false);

					if(readResult.EndOfStream)
						throw new System.Runtime.Serialization.SerializationException("Unexpected end of stream inside escaped cell");

					currentPosition = readResult.FoundPosition;

					//Check next char
					readResult = await GetCharAsync(currentPosition, 1, cancellationToken)
						.ConfigureAwait(false);

					if(readResult.EndOfStream || readResult.Character!=EscapeChar)
						break;

					currentPosition = readResult.FoundPosition;

					//Add skip position
					SkipPositions.Add(currentPosition);
				}

				//Make sure of proper chars after escape
				if(readResult.EndOfStream)
				{
					NewCurrentOffset = (int)(readResult.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition);
					NextNodeIsCell = false;
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition+1, currentPosition, SkipPositions, NodeType.Cell);
				}

				if(readResult.Character==DelimiterChar)
				{
					NewCurrentOffset = (int)(readResult.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition) + 1;
					NextNodeIsCell = true;
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition+1, currentPosition, SkipPositions, NodeType.Cell);
				}

				if(await IsProperNewLineAsync(readResult, cancellationToken).ConfigureAwait(false))
				{
					NewCurrentOffset = (int)(readResult.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition);
					NextNodeIsCell = false;
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition+1, currentPosition, SkipPositions, NodeType.Cell);
				}

				throw new System.Runtime.Serialization.SerializationException("Unexpected character after escaped cell");
			}

			//Find any of delimiter chars
			while(true)
			{
				found = await FindCharAsync(found.FoundPosition, 0, SearchArray, cancellationToken)
					.ConfigureAwait(false);

				//Check end of stream
				if(found.EndOfStream)
				{
					NewCurrentOffset = (int)(found.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition);
					NextNodeIsCell = false;
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
				}

				//Check delimiter
				if(found.Character==DelimiterChar)
				{
					NewCurrentOffset = (int)(found.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition) + 1;
					NextNodeIsCell = true;
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
				}

				//Check proper new line
				if(await IsProperNewLineAsync(found, cancellationToken).ConfigureAwait(false))
				{
					NewCurrentOffset = (int)(found.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition);

					if(NewCurrentOffset<=0 && !NextNodeIsCell)//New line
					{
						NewCurrentOffset = LineEnding==LineEnding.CRLF ? 2 : 1;
						NextNodeIsCell = !PermitEmptyLineAtEnd;
						return new MemorySequenceNode(CharMemorySequence.CurrentPosition, found.FoundPosition.AddOffset(NewCurrentOffset), Array.Empty<MemorySequencePosition<char>>(), NodeType.NewLine);
					}
					else//Cell ended by new line
					{
						NextNodeIsCell = false;
						return new MemorySequenceNode(CharMemorySequence.CurrentPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
					}
				}
			}
		}

		private async ValueTask<bool> IsProperNewLineAsync(ReadCharResult charRead, CancellationToken cancellationToken)
		{
			bool properLineEnding = false;

			switch(charRead.Character)
			{
				case '\r':
					ReadCharResult found;

					switch(LineEnding)
					{
						case LineEnding.CR:
							properLineEnding = true;
							break;
						case LineEnding.CRLF:
							found = await GetCharAsync(charRead.FoundPosition, 1, cancellationToken)
								.ConfigureAwait(false);
							properLineEnding = found.Character=='\n';//Don't have to check end of stream - found.Character is '\0' if end of stream
							break;
						case LineEnding.Auto:
							found = await GetCharAsync(charRead.FoundPosition, 1, cancellationToken)
								.ConfigureAwait(false);
							SearchArray = SearchArray.Slice(0, SearchArray.Length-1);//Change SearchArray to search only delimiter and '\r'
							if(found.Character=='\n')//Don't have to check end of stream - found.Character is '\0' if end of stream
								LineEnding = LineEnding.CRLF;
							else
								LineEnding = LineEnding.CR;
							properLineEnding = true;
							break;
					}
					break;
				case '\n':
					switch(LineEnding)
					{
						case LineEnding.LF:
							properLineEnding = true;
							break;
						case LineEnding.Auto:
							SearchArray = SearchArray.Slice(0, SearchArray.Length-1);//Change SearchArray to search only delimiter and '\n'
							SearchArray.Span[SearchArray.Length-1] = '\n';
							LineEnding = LineEnding.LF;
							properLineEnding = true;
							break;
					}
					break;
			}

			return properLineEnding;
		}

		private async ValueTask<ReadCharResult> GetCharAsync(MemorySequencePosition<char> currentPosition, int offset, CancellationToken cancellationToken)
		{
			MemorySequenceSegment<char> currentSegment = currentPosition.InternalSequenceSegment;
			int readingPositionInSegment = currentPosition.PositionInSegment + offset;

			Debug.Assert(offset>=0, $"{nameof(offset)} cannot be negative");

			//If current position isn't loaded, load it
			while(currentSegment.Memory.Length<=readingPositionInSegment)
			{
				var readSpan = await ReadChunkAsync(cancellationToken)
					.ConfigureAwait(false);

				//Check end of stream
				if(readSpan.Length<=0)
					return new ReadCharResult(new MemorySequencePosition<char>(readSpan.Segment, readSpan.Start), default, true);

				//If segment has changed, substract length of previous segment
				readingPositionInSegment -= (int)(readSpan.Segment.RunningIndex-currentSegment.RunningIndex);
				currentSegment = readSpan.Segment;
			}

			return new ReadCharResult(new MemorySequencePosition<char>(currentSegment, readingPositionInSegment), currentSegment.Array[readingPositionInSegment], false);
		}

		private async ValueTask<ReadCharResult> FindCharAsync(MemorySequencePosition<char> currentPosition, int offset, ReadOnlyMemory<char> charsToFind, CancellationToken cancellationToken)
		{
			MemorySequenceSegment<char> currentSegment = currentPosition.InternalSequenceSegment;
			int readingPositionInSegment = currentPosition.PositionInSegment + offset, indexOfFound;

			Debug.Assert(offset>=0, $"{nameof(offset)} cannot be negative");

			//Try find any of searching chars in current chunk of data
			while(0>(indexOfFound=currentSegment.Memory.Span.Slice(Math.Min(readingPositionInSegment, currentSegment.Memory.Length)).IndexOfAny(charsToFind.Span)))
			{
				readingPositionInSegment += Math.Max(currentSegment.Memory.Length-readingPositionInSegment, 0);

				//Searching chars has not been found in current chunk of data, so load next chunk
				var readSpan = await ReadChunkAsync(cancellationToken)
					.ConfigureAwait(false);

				//Check end of stream
				if(readSpan.Length<=0)
					return new ReadCharResult(new MemorySequencePosition<char>(readSpan.Segment, readSpan.Start), default, true);

				//If segment has changed, substract length of previous segment
				readingPositionInSegment -= (int)(readSpan.Segment.RunningIndex-currentSegment.RunningIndex);
				currentSegment = readSpan.Segment;
			}

			//Char found
			indexOfFound += readingPositionInSegment;
			return new ReadCharResult(new MemorySequencePosition<char>(currentSegment, indexOfFound), currentSegment.Memory.Span[indexOfFound], false);
		}

		/// <summary>
		/// Method read next chunk of data
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Chunk of data read</returns>
		private async ValueTask<MemorySequenceSegmentSpan<char>> ReadChunkAsync(CancellationToken cancellationToken)
		{
			//Is it very first load
			if(this.CurrentlyLoadingSegment==null)
			{
				//Start load data to first segment
				this.CurrentlyLoadingSegment = this.CharMemorySequence.CurrentPosition.InternalSequenceSegment;
#if NETSTANDARD2_1_OR_GREATER
				this.LoadingTask = this.TextReader.ReadAsync(this.CurrentlyLoadingSegment.Array, cancellationToken);
#else
				cancellationToken.ThrowIfCancellationRequested();
				this.LoadingTask = this.TextReader.ReadAsync(this.CurrentlyLoadingSegment.Array, 0, this.CurrentlyLoadingSegment.Array.Length);
#endif
			}

			//Wait for current segment to load
			var charsRead = await LoadingTask!
				.ConfigureAwait(false);

			var segmentRead = new MemorySequenceSegmentSpan<char>(this.CurrentlyLoadingSegment, this.CurrentlyLoadingSegment.Memory.Length, charsRead);

			//Check EndOfStream
			if(0<charsRead)
			{
				this.CurrentlyLoadingSegment.Count += charsRead;

				//Change segment if there is no free space in current segment
				if(this.CurrentlyLoadingSegment.Array.Length<=this.CurrentlyLoadingSegment.Memory.Length)
				{
					//Ensure next segment exists
					if(this.CurrentlyLoadingSegment.NextInternal==null)
						this.CharMemorySequence.AddNewSegment(minimumLength: this.BufferSizeInChars);

					//Start next loading task
					this.CurrentlyLoadingSegment = this.CurrentlyLoadingSegment.NextInternal;
				}

				//Start read next chunk
#if NETSTANDARD2_1_OR_GREATER
				this.LoadingTask = this.TextReader.ReadAsync(this.CurrentlyLoadingSegment!.Array.AsMemory(this.CurrentlyLoadingSegment.Memory.Length), cancellationToken);
#else
				cancellationToken.ThrowIfCancellationRequested();
				this.LoadingTask = this.TextReader.ReadAsync(this.CurrentlyLoadingSegment!.Array, this.CurrentlyLoadingSegment.Memory.Length, this.CurrentlyLoadingSegment.Array.Length-this.CurrentlyLoadingSegment.Memory.Length);
#endif
			}
#if NETSTANDARD2_1_OR_GREATER
			else
			{
				//ValueTask can be awaited only once so create new completed ValueTask in case of await again
				this.LoadingTask = new ValueTask<int>(0);
			}
#endif

			return segmentRead;
		}

		public void Dispose()
		{
			if(!LeaveOpen)
				TextReader.Dispose();

#if NETSTANDARD2_1_OR_GREATER
			if(!LoadingTask.IsCompleted)
			{
				try
				{
					_ = LoadingTask.Result;
				}
				catch(Exception)
				{ }
				//ValueTask can be awaited only once so create new completed ValueTask in case of await again
				LoadingTask = new ValueTask<int>(0);
			}
#else
			if(LoadingTask!=null && !LoadingTask.IsCompleted)
			{
				try
				{
					LoadingTask.Wait();
				}
				catch(Exception)
				{ }
			}
#endif

			this.SkipPositions.Clear();

			if(TempCellArray!=null)
			{
				ArrayPool<char>.Shared.Return(TempCellArray, true);
				TempCellArray = null;
			}
			CharMemorySequence.Dispose();
		}
	}
}