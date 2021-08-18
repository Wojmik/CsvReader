using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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

		public bool HeaderRow { get; }

		private ReadOnlyMemory<char> EscapeCharArray;

		private Memory<char> SearchArray;

		private MemorySequence<char> CharMemorySequence;

		private int NewCurrentOffset;

		private MemorySequenceSegment<char> CurrentlyLoadingSegment;

#if NETSTANDARD2_1_OR_GREATER
		ValueTask<int> LoadingTask;
#else
		Task<int> LoadingTask;
#endif

		private readonly List<MemorySequencePosition<char>> SkipPositions;

		private int BufferSizeInChars { get; }

		public long Position { get => this.CharMemorySequence.CurrentPosition.AbsolutePosition; }

		private readonly bool LeaveOpen;

		public CsvReader(TextReader textReader)
			: this(textReader: textReader, options: new CsvReaderOptions())
		{ }

		public CsvReader(TextReader textReader, CsvReaderOptions options)
		{
			const int minBufferSize = 1;

			if(textReader==null)
				throw new ArgumentNullException(nameof(textReader));
			if(options.BufferSizeInChars<minBufferSize)
				throw new ArgumentOutOfRangeException($"{nameof(options)}.{nameof(options.BufferSizeInChars)}", options.BufferSizeInChars, $"{nameof(options)}.{nameof(options.BufferSizeInChars)} cannot be less than {minBufferSize}");

			this.TextReader = textReader;
			this.CanEscape = options.CanEscape;
			this.EscapeChar = options.EscapeChar;
			this.DelimiterChar = options.DelimiterChar;
			this.LineEnding = options.LineEnding;
			this.BufferSizeInChars=options.BufferSizeInChars;
			this.LeaveOpen = options.LeaveOpen;

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

#if NETSTANDARD2_1_OR_GREATER
		public async IAsyncEnumerable<int> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			await Task.Yield();
			yield return 5;
		}
#endif

		private async ValueTask Yh()
		{
			
			//var a= PipeReader.Create(TextReader);

			//new SequenceReader<char>()
			//new ReadOnlySequenceSegment<char>()
		}

		/// <summary>
		/// Method reads next csv node as memory sequence
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <exception cref="System.Runtime.Serialization.SerializationException">Broken csv detected. End of stream inside escaped cell or data after end of escaped cell</exception>
		/// <returns>Next csv node read</returns>
		public async ValueTask<MemorySequenceNode> ReadNextNodeAsMemorySequenceAsync(CancellationToken cancellationToken)
		{
			CharMemorySequence.MoveForward(CharMemorySequence.CurrentPosition, NewCurrentOffset);
			NewCurrentOffset = 0;

			var found = await GetCharAsync(CharMemorySequence.CurrentPosition, 0, cancellationToken)
				.ConfigureAwait(false);

			//Check end of stream
			if(found.EndOfStream)
				return new MemorySequenceNode(found.FoundPosition, found.FoundPosition, NodeType.EndOfStream, Array.Empty<MemorySequencePosition<char>>());

			//Check delimiter shortcut
			if(found.Character==DelimiterChar)
			{
				NewCurrentOffset = 1;
				return new MemorySequenceNode(found.FoundPosition, found.FoundPosition, NodeType.Cell, Array.Empty<MemorySequencePosition<char>>());
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
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition+1, currentPosition, NodeType.Cell, SkipPositions);
				}

				if(readResult.Character==DelimiterChar)
				{
					NewCurrentOffset = (int)(readResult.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition) + 1;
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition+1, currentPosition, NodeType.Cell, SkipPositions);
				}

				if(await IsProperNewLineAsync(readResult, cancellationToken).ConfigureAwait(false))
				{
					NewCurrentOffset = (int)(readResult.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition);
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition+1, currentPosition, NodeType.Cell, SkipPositions);
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
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition, found.FoundPosition, NodeType.Cell, Array.Empty<MemorySequencePosition<char>>());
				}

				//Check delimiter
				if(found.Character==DelimiterChar)
				{
					NewCurrentOffset = (int)(found.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition) + 1;
					return new MemorySequenceNode(CharMemorySequence.CurrentPosition, found.FoundPosition, NodeType.Cell, Array.Empty<MemorySequencePosition<char>>());
				}

				//Check proper new line
				if(await IsProperNewLineAsync(found, cancellationToken).ConfigureAwait(false))
				{
					NewCurrentOffset = (int)(found.FoundPosition.AbsolutePosition-CharMemorySequence.CurrentPosition.AbsolutePosition);

					if(NewCurrentOffset<=0)//New line
					{
						NewCurrentOffset = LineEnding==LineEnding.CRLF ? 2 : 1;
						return new MemorySequenceNode(CharMemorySequence.CurrentPosition, found.FoundPosition, NodeType.NewLine, Array.Empty<MemorySequencePosition<char>>());
					}
					else//Cell ended by new line
						return new MemorySequenceNode(CharMemorySequence.CurrentPosition, found.FoundPosition, NodeType.Cell, Array.Empty<MemorySequencePosition<char>>());
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
			var charsRead = await LoadingTask
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
				this.LoadingTask = this.TextReader.ReadAsync(this.CurrentlyLoadingSegment.Array.AsMemory(this.CurrentlyLoadingSegment.Memory.Length), cancellationToken);
#else
				cancellationToken.ThrowIfCancellationRequested();
				this.LoadingTask = this.TextReader.ReadAsync(this.CurrentlyLoadingSegment.Array, this.CurrentlyLoadingSegment.Memory.Length, this.CurrentlyLoadingSegment.Array.Length-this.CurrentlyLoadingSegment.Memory.Length);
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
			if(!LoadingTask.IsCompleted)
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

			CharMemorySequence.Dispose();
		}
	}
}