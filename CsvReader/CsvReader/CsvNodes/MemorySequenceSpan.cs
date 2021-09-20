using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader.CsvNodes
{
	public readonly struct MemorySequenceSpan
	{
		public readonly MemorySequencePosition<char> StartPosition;
		
		public readonly MemorySequencePosition<char> EndPosition;

		public readonly IReadOnlyList<MemorySequencePosition<char>> SkipCharPositions;

		public int CharsCount { get => (int)(EndPosition.AbsolutePosition-StartPosition.AbsolutePosition)-SkipCharPositions.Count; }

		public MemorySequenceSpan(in MemorySequencePosition<char> startPosition, in MemorySequencePosition<char> endPosition, IReadOnlyList<MemorySequencePosition<char>> skipCharPositions)
		{
			this.StartPosition=startPosition;
			this.EndPosition=endPosition;
			this.SkipCharPositions=skipCharPositions;
		}

		public int CopyDataTo(Span<char> destination)
		{
			//Copy cell data to destination continous memory region
			var skipCharPositions = SkipCharPositions;
			var segment = StartPosition.SequenceSegment;
			int sourceIndex = StartPosition.PositionInSegment, destinationIndex = 0, skipIndex = 0;
			while(!ReferenceEquals(segment, EndPosition.SequenceSegment))
			{
				CopySegment(destination, segment.Memory.Length);

				segment = segment.Next;
				sourceIndex = 0;
			}
			CopySegment(destination, EndPosition.PositionInSegment);

			return destinationIndex;

			void CopySegment(Span<char> in_destination, int segmentLength)
			{
				int chunkLength;

				//Copy chunks between skips
				while(skipIndex<skipCharPositions.Count && ReferenceEquals(segment, skipCharPositions[skipIndex].SequenceSegment))
				{
					chunkLength = skipCharPositions[skipIndex].PositionInSegment-sourceIndex;
					segment.Memory.Span
						.Slice(sourceIndex, chunkLength)
						.CopyTo(in_destination.Slice(destinationIndex));

					sourceIndex += chunkLength+1;
					destinationIndex += chunkLength;

					skipIndex++;
				}

				//Copy last chunk in this segment
				chunkLength = segmentLength-sourceIndex;
				segment.Memory.Span
					.Slice(sourceIndex, chunkLength)
					.CopyTo(in_destination.Slice(destinationIndex));

				destinationIndex += chunkLength;
			}
		}
	}
}