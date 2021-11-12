using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader.CsvNodes
{
	/// <summary>
	/// Span of <see cref="System.Buffers.ReadOnlySequenceSegment{T}"/> containing csv cell data
	/// </summary>
	public readonly struct MemorySequenceSpan
	{
		/// <summary>
		/// Start position
		/// </summary>
		public readonly MemorySequencePosition<char> StartPosition;
		
		/// <summary>
		/// End position
		/// </summary>
		public readonly MemorySequencePosition<char> EndPosition;

		/// <summary>
		/// Positions of chars to skip
		/// </summary>
		public readonly IReadOnlyList<MemorySequencePosition<char>> SkipCharPositions;

		/// <summary>
		/// Number of valid chars (without skiped chars) contained in this <see cref="MemorySequenceSpan"/>
		/// </summary>
		public int CharsCount { get => (int)(EndPosition.AbsolutePosition-StartPosition.AbsolutePosition)-SkipCharPositions.Count; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="startPosition">Start position</param>
		/// <param name="endPosition">End position</param>
		/// <param name="skipCharPositions">Positions of chars to skip</param>
		public MemorySequenceSpan(in MemorySequencePosition<char> startPosition, in MemorySequencePosition<char> endPosition, IReadOnlyList<MemorySequencePosition<char>> skipCharPositions)
		{
			this.StartPosition=startPosition;
			this.EndPosition=endPosition;
			this.SkipCharPositions=skipCharPositions;
		}

		/// <summary>
		/// Copies csv cell data contained in this <see cref="MemorySequenceSpan"/> to continues memory region throwing out skip characters
		/// </summary>
		/// <param name="destination">Destination to copy cell data contained in this <see cref="MemorySequenceSpan"/> to</param>
		/// <returns>Number of chars copied</returns>
		public int CopyDataTo(Span<char> destination)
		{
			//Copy cell data to destination continous memory region
			var skipCharPositions = SkipCharPositions;
			var segment = StartPosition.SequenceSegment;
			int sourceIndex = StartPosition.PositionInSegment, destinationIndex = 0, skipIndex = 0;
			while(!ReferenceEquals(segment, EndPosition.SequenceSegment))
			{
				CopySegment(destination, segment!.Memory.Length);

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