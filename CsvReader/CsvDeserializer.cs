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
	public class CsvDeserializer : IDisposable
	{
		private const int DefaultBufferSizeInChars = 32 * 1024;

		private TextReader TextReader { get; }

		public char EscapeChar { get; }

		public char Delimiter { get; }

		public LineEnding LineEnding { get; private set; }

		public bool CanEscape { get; }

		public bool HeaderRow { get; }

		private readonly bool SecondLineEnding;

		private readonly char[] _SearchArray;

		private ReadOnlyMemory<char> SearchArray;

		private MemorySequence<char> CharMemorySequence;

		private int CurrentOffset;

		private MemorySequenceSegment<char> CurrentlyLoadingSegment;

#if NETSTANDARD2_1_OR_GREATER
		ValueTask<int> LoadingTask;
#else
		Task<int> LoadingTask;
#endif

		private int BufferSizeInChars { get; }

		public long Position { get => this.CharMemorySequence.CurrentPosition.AbsolutePosition; }

		private readonly bool LeaveOpen;

		public CsvDeserializer(
			TextReader textReader,
			bool headerRow,
			bool canEscape = true,
			char escapeCharacter = '\"',
			char delimiter = ',',
			LineEnding lineEnding = LineEnding.Auto,
			int bufferSize = DefaultBufferSizeInChars,
			bool leaveOpen = false
			)
		{
			if(textReader==null)
				throw new ArgumentNullException(nameof(textReader));

			this.TextReader = textReader;
			this.HeaderRow = headerRow;
			this.CanEscape = canEscape;
			this.EscapeChar = escapeCharacter;
			this.Delimiter = delimiter;
			this.LineEnding = lineEnding;
			this.BufferSizeInChars=bufferSize;
			this.LeaveOpen = leaveOpen;

			//Create data for SearchArray
			_SearchArray = new char[2+(canEscape ? 1 : 0)+(lineEnding==LineEnding.Auto || lineEnding==LineEnding.CRLF ? 1 : 0)];
			int i = 0;
			if(canEscape)
				_SearchArray[i++] = escapeCharacter;
			_SearchArray[i++] = delimiter;
			switch(lineEnding)
			{
				case LineEnding.Auto:
				case LineEnding.CRLF:
					_SearchArray[i++] = '\r';
					_SearchArray[i++] = '\n';
					break;
				case LineEnding.LF:
					_SearchArray[i++] = '\n';
					break;
				case LineEnding.CR:
					_SearchArray[i++] = '\r';
					break;
				default:
					throw new ArgumentException("Invalid line ending", nameof(lineEnding));
			}
			SearchArray = _SearchArray;

			//Add first segment to CharMemorySequence
			this.CharMemorySequence.AddNewSegment(minimumLength: this.BufferSizeInChars);
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

		private async ValueTask<MemorySequenceNode> ReadNextMemorySequenceNodeAsync(CancellationToken cancellationToken)
		{
			var found = await GetCharAsync(CharMemorySequence.CurrentPosition, CurrentOffset, cancellationToken)
				.ConfigureAwait(false);

			//Check delimiter shortcut
			if(found.Character==Delimiter)
			{
				CurrentOffset = 1;
				return new MemorySequenceNode(found.FoundPosition, found.FoundPosition, NodeType.Cell, 0);
			}

			if(found.Character==EscapeChar)
			{

			}

			//Is it EndOfStream
			if(found.EndOfStream)
				return new MemorySequenceNode(startPosition: this.CharMemorySequence.CurrentPosition, endPosition: found.FoundPosition, nodeType: NodeType.EndOfStream, escapeCount: 0);

			//It is not EndOfStream
			if(found.Character==this.EscapeChar)
			{

			}


			throw new NotImplementedException();
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

			var segment = CharMemorySequence.CurrentPosition.InternalSequenceSegment;
			while(segment!=null)
			{
				segment.Dispose();
				segment = segment.NextInternal;
			}
		}
	}
}