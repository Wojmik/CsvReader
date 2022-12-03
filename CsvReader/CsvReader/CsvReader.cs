using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader
{
	/// <summary>
	/// Low level csv reader. This object returns individual csv nodes in one of three maner
	/// </summary>
	public class CsvReader : IDisposable
	{
		private readonly TextReader _textReader;

		/// <summary>
		/// Escape character
		/// </summary>
		public char EscapeChar { get; }

		/// <summary>
		/// Delimiter character
		/// </summary>
		public char DelimiterChar { get; }

		/// <summary>
		/// Line ending
		/// </summary>
		public LineEnding LineEnding { get; private set; }

		/// <summary>
		/// Can csv cells be escaped
		/// </summary>
		public bool CanEscape { get; }

		/// <summary>
		/// Permits empty line at the end of the csv stream
		/// </summary>
		public bool PermitEmptyLineAtEnd { get; }

		private readonly ReadOnlyMemory<char> _escapeCharArray;

		private Memory<char> _searchArray;

		private MemorySequence<char> _charMemorySequence;

		private int _newCurrentOffset;

		private MemorySequenceSegment<char>? _currentlyLoadingSegment;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		ValueTask<int> _loadingTask;
#else
		Task<int>? _loadingTask;
#endif

		private readonly List<MemorySequencePosition<char>> _skipPositions;

		private int BufferSizeInChars { get; }

		/// <summary>
		/// Current position in cvs stream
		/// </summary>
		public long Position { get => _charMemorySequence.CurrentPosition.AbsolutePosition + _newCurrentOffset; }

		private long _loadedChars;

		private readonly bool _leaveOpen;

		private char[]? _tempCellArray;

		private bool _nextNodeIsCell;

		private static ICsvReaderOptions CreateOptions(Action<ICsvReaderOptions>? optionsDelegate)
		{
			var options = new CsvReaderOptions();
			optionsDelegate?.Invoke(options);
			return options;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textReader">Text reader to read csv nodes from</param>
		/// <param name="optionsDelegate">Method to configure options</param>
		/// <exception cref="ArgumentNullException"><paramref name="textReader"/> is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">Buffer size in chars is too small</exception>
		public CsvReader(TextReader textReader, Action<ICsvReaderOptions>? optionsDelegate = null)
			: this(textReader, CreateOptions(optionsDelegate))
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textReader">Text reader to read csv nodes from</param>
		/// <param name="options">Options object</param>
		/// <exception cref="ArgumentNullException"><paramref name="textReader"/> or <paramref name="options"/> is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">Buffer size in chars is too small</exception>
		internal CsvReader(TextReader textReader, ICsvReaderOptions options)
		{
			const int MinBufferSize = 1;

			if (options == null)
				throw new ArgumentNullException(nameof(options));

			if (options.BufferSizeInChars < MinBufferSize)
				throw new ArgumentOutOfRangeException($"{nameof(options)}.{nameof(options.BufferSizeInChars)}", options.BufferSizeInChars, $"{nameof(options)}.{nameof(options.BufferSizeInChars)} cannot be less than {MinBufferSize}");

			_textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
			CanEscape = options.CanEscape;
			PermitEmptyLineAtEnd = options.PermitEmptyLineAtEnd;
			EscapeChar = options.EscapeChar;
			DelimiterChar = options.DelimiterChar;
			LineEnding = options.LineEnding;
			BufferSizeInChars = options.BufferSizeInChars;
			_leaveOpen = options.LeaveOpen;

			_nextNodeIsCell = true;

			//Create data for SearchArray
			var searchArray = new char[2 + (CanEscape ? 1 : 0) + (LineEnding == LineEnding.Auto ? 1 : 0)];
			int i = 0;
			if (CanEscape)
			{
				searchArray[i++] = EscapeChar;
				_escapeCharArray = new ReadOnlyMemory<char>(searchArray, 0, 1);
			}
			_searchArray = new Memory<char>(searchArray, i, searchArray.Length - i);
			searchArray[i++] = DelimiterChar;
			switch (LineEnding)
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
			_charMemorySequence.AddNewSegment(BufferSizeInChars);

			_skipPositions = new List<MemorySequencePosition<char>>();
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
			if (0 < charsCount)
			{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
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
		/// Method process <paramref name="memorySequenceSpan"/> to continous memory region and returns <see cref="ReadOnlyMemory{T}"/>.
		/// Returned <see cref="ReadOnlyMemory{T}"/> is valid only till next ReadNextNodeAsXxx call.
		/// </summary>
		/// <param name="memorySequenceSpan">Memory sequence span to process</param>
		/// <returns>Continous memory region of chars</returns>
		public ReadOnlyMemory<char> MemorySequenceAsMemory(in MemorySequenceSpan memorySequenceSpan)
		{
			//Check is it single segment
			if (ReferenceEquals(memorySequenceSpan.StartPosition.SequenceSegment, memorySequenceSpan.EndPosition.SequenceSegment))
			{
				//Single segment, only sliding is needed
				if (0 < memorySequenceSpan.SkipCharPositions.Count)
				{
					int i, startHole, endHole;
					endHole = memorySequenceSpan.SkipCharPositions[0].PositionInSegment;
					for (i = 1; i < memorySequenceSpan.SkipCharPositions.Count; i++)
					{
						startHole = endHole + 1;
						endHole = memorySequenceSpan.SkipCharPositions[i].PositionInSegment;
						Array.Copy(memorySequenceSpan.StartPosition.InternalSequenceSegment.Array, startHole, memorySequenceSpan.StartPosition.InternalSequenceSegment.Array, startHole - i, endHole - startHole);
					}
					startHole = endHole + 1;
					endHole = memorySequenceSpan.EndPosition.PositionInSegment;
					Array.Copy(memorySequenceSpan.StartPosition.InternalSequenceSegment.Array, startHole, memorySequenceSpan.StartPosition.InternalSequenceSegment.Array, startHole - i, endHole - startHole);
				}
				return memorySequenceSpan.StartPosition.SequenceSegment.Memory.Slice(memorySequenceSpan.StartPosition.PositionInSegment, memorySequenceSpan.EndPosition.PositionInSegment - memorySequenceSpan.StartPosition.PositionInSegment - memorySequenceSpan.SkipCharPositions.Count);
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

			if (requiredSize <= sequenceSegment.Array.Length)//Sequence segment is big enough to fit cell data
				destination = sequenceSegment.Array;
			else
			{
				if (_tempCellArray == null)
					_tempCellArray = ArrayPool<char>.Shared.Rent(requiredSize);
				else if (_tempCellArray.Length < requiredSize)
				{
					ArrayPool<char>.Shared.Return(_tempCellArray, true);
					_tempCellArray = ArrayPool<char>.Shared.Rent(requiredSize);
				}
				destination = _tempCellArray;
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
			_charMemorySequence.MoveForward(_charMemorySequence.CurrentPosition, _newCurrentOffset);
			_newCurrentOffset = 0;

			var found = await GetCharAsync(_charMemorySequence.CurrentPosition, 0, cancellationToken)
				.ConfigureAwait(false);

			//Check end of stream
			if (found.EndOfStream)
			{
				if (_nextNodeIsCell && 0 < found.FoundPosition.AbsolutePosition)//If file is empty it should return only end of stream
				{
					_nextNodeIsCell = false;
					return new MemorySequenceNode(_charMemorySequence.CurrentPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
				}
				return new MemorySequenceNode(found.FoundPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.EndOfStream);
			}

			//Check delimiter shortcut
			if (found.Character == DelimiterChar)
			{
				_newCurrentOffset = 1;
				_nextNodeIsCell = true;
				return new MemorySequenceNode(found.FoundPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
			}

			//Check start of escaped cell
			if (found.Character == EscapeChar && CanEscape)
			{
				var currentPosition = _charMemorySequence.CurrentPosition;
				ReadCharResult readResult;

				_skipPositions.Clear();

				while (true)
				{
					readResult = await FindCharAsync(currentPosition, 1, _escapeCharArray, cancellationToken)
						.ConfigureAwait(false);

					if (readResult.EndOfStream)
						throw new System.Runtime.Serialization.SerializationException("Unexpected end of stream inside escaped cell");

					currentPosition = readResult.FoundPosition;

					//Check next char
					readResult = await GetCharAsync(currentPosition, 1, cancellationToken)
						.ConfigureAwait(false);

					if (readResult.EndOfStream || readResult.Character != EscapeChar)
						break;

					currentPosition = readResult.FoundPosition;

					//Add skip position
					_skipPositions.Add(currentPosition);
				}

				//Make sure of proper chars after escape
				if (readResult.EndOfStream)
				{
					_newCurrentOffset = (int)(readResult.FoundPosition.AbsolutePosition - _charMemorySequence.CurrentPosition.AbsolutePosition);
					_nextNodeIsCell = false;
					return new MemorySequenceNode(_charMemorySequence.CurrentPosition + 1, currentPosition, _skipPositions, NodeType.Cell);
				}

				if (readResult.Character == DelimiterChar)
				{
					_newCurrentOffset = (int)(readResult.FoundPosition.AbsolutePosition - _charMemorySequence.CurrentPosition.AbsolutePosition) + 1;
					_nextNodeIsCell = true;
					return new MemorySequenceNode(_charMemorySequence.CurrentPosition + 1, currentPosition, _skipPositions, NodeType.Cell);
				}

				if (await IsProperNewLineAsync(readResult, cancellationToken).ConfigureAwait(false))
				{
					_newCurrentOffset = (int)(readResult.FoundPosition.AbsolutePosition - _charMemorySequence.CurrentPosition.AbsolutePosition);
					_nextNodeIsCell = false;
					return new MemorySequenceNode(_charMemorySequence.CurrentPosition + 1, currentPosition, _skipPositions, NodeType.Cell);
				}

				throw new System.Runtime.Serialization.SerializationException("Unexpected character after escaped cell");
			}

			//Find any of delimiter chars
			while (true)
			{
				found = await FindCharAsync(found.FoundPosition, 0, _searchArray, cancellationToken)
					.ConfigureAwait(false);

				//Check end of stream
				if (found.EndOfStream)
				{
					_newCurrentOffset = (int)(found.FoundPosition.AbsolutePosition - _charMemorySequence.CurrentPosition.AbsolutePosition);
					_nextNodeIsCell = false;
					return new MemorySequenceNode(_charMemorySequence.CurrentPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
				}

				//Check delimiter
				if (found.Character == DelimiterChar)
				{
					_newCurrentOffset = (int)(found.FoundPosition.AbsolutePosition - _charMemorySequence.CurrentPosition.AbsolutePosition) + 1;
					_nextNodeIsCell = true;
					return new MemorySequenceNode(_charMemorySequence.CurrentPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
				}

				//Check proper new line
				if (await IsProperNewLineAsync(found, cancellationToken).ConfigureAwait(false))
				{
					_newCurrentOffset = (int)(found.FoundPosition.AbsolutePosition - _charMemorySequence.CurrentPosition.AbsolutePosition);

					if (_newCurrentOffset <= 0 && !_nextNodeIsCell)//New line
					{
						_newCurrentOffset = LineEnding == LineEnding.CRLF ? 2 : 1;
						_nextNodeIsCell = !PermitEmptyLineAtEnd;
						return new MemorySequenceNode(_charMemorySequence.CurrentPosition, found.FoundPosition.AddOffset(_newCurrentOffset), Array.Empty<MemorySequencePosition<char>>(), NodeType.NewLine);
					}
					else//Cell ended by new line
					{
						_nextNodeIsCell = false;
						return new MemorySequenceNode(_charMemorySequence.CurrentPosition, found.FoundPosition, Array.Empty<MemorySequencePosition<char>>(), NodeType.Cell);
					}
				}
			}
		}

		private async ValueTask<bool> IsProperNewLineAsync(ReadCharResult charRead, CancellationToken cancellationToken)
		{
			bool properLineEnding = false;

			switch (charRead.Character)
			{
				case '\r':
					ReadCharResult found;

					switch (LineEnding)
					{
						case LineEnding.CR:
							properLineEnding = true;
							break;
						case LineEnding.CRLF:
							found = await GetCharAsync(charRead.FoundPosition, 1, cancellationToken)
								.ConfigureAwait(false);
							properLineEnding = found.Character == '\n';//Don't have to check end of stream - found.Character is '\0' if end of stream
							break;
						case LineEnding.Auto:
							found = await GetCharAsync(charRead.FoundPosition, 1, cancellationToken)
								.ConfigureAwait(false);
							_searchArray = _searchArray.Slice(0, _searchArray.Length - 1);//Change SearchArray to search only delimiter and '\r'
							if (found.Character == '\n')//Don't have to check end of stream - found.Character is '\0' if end of stream
								LineEnding = LineEnding.CRLF;
							else
								LineEnding = LineEnding.CR;
							properLineEnding = true;
							break;
					}
					break;
				case '\n':
					switch (LineEnding)
					{
						case LineEnding.LF:
							properLineEnding = true;
							break;
						case LineEnding.Auto:
							_searchArray = _searchArray.Slice(0, _searchArray.Length - 1);//Change SearchArray to search only delimiter and '\n'
							_searchArray.Span[_searchArray.Length - 1] = '\n';
							LineEnding = LineEnding.LF;
							properLineEnding = true;
							break;
					}
					break;
			}

			return properLineEnding;
		}

		//private async ValueTask<ReadCharResult> GetCharAsync(MemorySequenceSegment<char> currentSegment, long position, CancellationToken cancellationToken)
		//{
		//	int positionInSegment = (int)(position - currentSegment.RunningIndex);

		//	while(currentSegment.Memory.Length<=positionInSegment)
		//	{
		//		if(LoadedChars<=position)
		//		{
		//			var readSpan = await ReadChunkAsync(cancellationToken)
		//			.ConfigureAwait(false);

		//			//Check end of stream
		//			if(readSpan.Length<=0)
		//				return new ReadCharResult(new MemorySequencePosition<char>(readSpan.Segment, readSpan.Start), default, true);
		//		}

		//		if(currentSegment.Array.Length<=currentSegment.Memory.Length)
		//		{
		//			positionInSegment -= currentSegment.Memory.Length;
		//			currentSegment = currentSegment.NextInternal!;
		//		}
		//	}

		//	return new ReadCharResult(new MemorySequencePosition<char>(currentSegment, positionInSegment), currentSegment.Array[positionInSegment], false);
		//}

		private async ValueTask<ReadCharResult> GetCharAsync(MemorySequencePosition<char> currentPosition, int offset, CancellationToken cancellationToken)
		{
			MemorySequenceSegment<char> currentSegment = currentPosition.InternalSequenceSegment;
			int readingPositionInSegment = currentPosition.PositionInSegment + offset;

			Debug.Assert(offset >= 0, $"{nameof(offset)} cannot be negative");

			//If current position isn't loaded, load it
			while (currentSegment.Memory.Length <= readingPositionInSegment)
			{
				if (_loadedChars <= currentSegment.RunningIndex + readingPositionInSegment)
				{
					var readSpan = await ReadChunkAsync(cancellationToken)
						.ConfigureAwait(false);

					//Check end of stream
					if (readSpan.Length <= 0)
						return new ReadCharResult(new MemorySequencePosition<char>(readSpan.Segment, readSpan.Start), default, true);
				}

				//If segment has changed, substract length of previous segment
				if (currentSegment.Memory.Length <= readingPositionInSegment && currentSegment.Array.Length <= currentSegment.Memory.Length)
				{
					readingPositionInSegment -= currentSegment.Memory.Length;
					currentSegment = currentSegment.NextInternal!;
				}
			}

			return new ReadCharResult(new MemorySequencePosition<char>(currentSegment, readingPositionInSegment), currentSegment.Array[readingPositionInSegment], false);
		}

		private async ValueTask<ReadCharResult> FindCharAsync(MemorySequencePosition<char> currentPosition, int offset, ReadOnlyMemory<char> charsToFind, CancellationToken cancellationToken)
		{
			MemorySequenceSegment<char> currentSegment = currentPosition.InternalSequenceSegment;
			int readingPositionInSegment = currentPosition.PositionInSegment + offset, indexOfFound;

			Debug.Assert(offset >= 0, $"{nameof(offset)} cannot be negative");

			//Try find any of searching chars in current chunk of data
			while (0 > (indexOfFound = currentSegment.Memory.Span.Slice(Math.Min(readingPositionInSegment, currentSegment.Memory.Length)).IndexOfAny(charsToFind.Span)))
			{
				readingPositionInSegment += Math.Max(currentSegment.Memory.Length - readingPositionInSegment, 0);

				if (_loadedChars <= currentSegment.RunningIndex + readingPositionInSegment)
				{
					//Searching chars has not been found in current chunk of data, so load next chunk
					var readSpan = await ReadChunkAsync(cancellationToken)
						.ConfigureAwait(false);

					//Check end of stream
					if (readSpan.Length <= 0)
						return new ReadCharResult(new MemorySequencePosition<char>(readSpan.Segment, readSpan.Start), default, true);
				}

				//If segment has changed, substract length of previous segment
				if (currentSegment.Memory.Length <= readingPositionInSegment && currentSegment.Array.Length <= currentSegment.Memory.Length)
				{
					readingPositionInSegment -= currentSegment.Memory.Length;
					currentSegment = currentSegment.NextInternal!;
				}
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
			if (this._currentlyLoadingSegment == null)
			{
				//Start load data to first segment
				this._currentlyLoadingSegment = this._charMemorySequence.CurrentPosition.InternalSequenceSegment;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
				this._loadingTask = this._textReader.ReadAsync(this._currentlyLoadingSegment.Array, cancellationToken);
#else
				cancellationToken.ThrowIfCancellationRequested();
				this._loadingTask = this._textReader.ReadAsync(this._currentlyLoadingSegment.Array, 0, this._currentlyLoadingSegment.Array.Length);
#endif
			}

			//Wait for current segment to load
			var charsRead = await _loadingTask!
				.ConfigureAwait(false);

			var segmentRead = new MemorySequenceSegmentSpan<char>(this._currentlyLoadingSegment, this._currentlyLoadingSegment.Memory.Length, charsRead);

			//Check EndOfStream
			if (0 < charsRead)
			{
				this._currentlyLoadingSegment.Count += charsRead;
				this._loadedChars += charsRead;

				//Change segment if there is no free space in current segment
				if (this._currentlyLoadingSegment.Array.Length <= this._currentlyLoadingSegment.Memory.Length)
				{
					//Ensure next segment exists
					if (this._currentlyLoadingSegment.NextInternal == null)
						this._charMemorySequence.AddNewSegment(minimumLength: this.BufferSizeInChars);

					//Start next loading task
					this._currentlyLoadingSegment = this._currentlyLoadingSegment.NextInternal;
				}

				//Start read next chunk
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
				this._loadingTask = this._textReader.ReadAsync(this._currentlyLoadingSegment!.Array.AsMemory(this._currentlyLoadingSegment.Memory.Length), cancellationToken);
#else
				cancellationToken.ThrowIfCancellationRequested();
				this._loadingTask = this._textReader.ReadAsync(this._currentlyLoadingSegment!.Array, this._currentlyLoadingSegment.Memory.Length, this._currentlyLoadingSegment.Array.Length-this._currentlyLoadingSegment.Memory.Length);
#endif
			}
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
			else
			{
				//ValueTask can be awaited only once so create new completed ValueTask in case of await again
				this._loadingTask = new ValueTask<int>(0);
			}
#endif

			return segmentRead;
		}

		/// <summary>
		/// Disposes <see cref="CsvReader"/>
		/// </summary>
		public void Dispose()
		{
			if (!_leaveOpen)
				_textReader.Dispose();

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
			if (!_loadingTask.IsCompleted)
			{
				try
				{
					_ = _loadingTask.Result;
				}
				catch (Exception)
				{ }
				//ValueTask can be awaited only once so create new completed ValueTask in case of await again
				_loadingTask = new ValueTask<int>(0);
			}
#else
			if(_loadingTask!=null && !_loadingTask.IsCompleted)
			{
				try
				{
					_loadingTask.Wait();
				}
				catch(Exception)
				{ }
			}
#endif

			_skipPositions.Clear();

			var tempCellArray = _tempCellArray;
			if (tempCellArray != null)
			{
				ArrayPool<char>.Shared.Return(tempCellArray, true);
				_tempCellArray = null;
			}
			_charMemorySequence.Dispose();
		}
	}
}