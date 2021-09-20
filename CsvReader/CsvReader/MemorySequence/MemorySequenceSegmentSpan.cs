using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.MemorySequence
{
	readonly struct MemorySequenceSegmentSpan<T>
	{
		public readonly MemorySequenceSegment<T> Segment;

		public readonly int Start;

		public readonly int Length;

		public ReadOnlyMemory<T> Memory { get => Segment.Memory.Slice(Start, Length); }

		public MemorySequenceSegmentSpan(MemorySequenceSegment<T> segment, int start, int length)
		{
			this.Segment = segment;
			this.Start = start;
			this.Length = length;
		}
	}
}