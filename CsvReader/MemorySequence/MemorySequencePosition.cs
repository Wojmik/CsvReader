using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.MemorySequence
{
	public readonly struct MemorySequencePosition<T> : IEquatable<MemorySequencePosition<T>>
	{
		public readonly MemorySequenceSegment<T> SequenceSegment;

		public readonly int PositionInSegment;

		public long AbsolutePosition { get => this.SequenceSegment.RunningIndex+this.PositionInSegment; }

		public MemorySequencePosition(MemorySequenceSegment<T> sequenceSegment, int positionInSegment)
		{
			this.SequenceSegment=sequenceSegment;
			this.PositionInSegment=positionInSegment;
		}

		public override bool Equals(object obj)
		{
			return obj is MemorySequencePosition<T> position && Equals(position);
		}

		public bool Equals(MemorySequencePosition<T> other)
		{
			return PositionInSegment==other.PositionInSegment
				&& EqualityComparer<MemorySequenceSegment<T>>.Default.Equals(SequenceSegment, other.SequenceSegment);
		}

		public override int GetHashCode()
		{
			int hashCode = -1739113237;
			hashCode=hashCode*-1521134295+EqualityComparer<MemorySequenceSegment<T>>.Default.GetHashCode(SequenceSegment);
			hashCode=hashCode*-1521134295+PositionInSegment.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(MemorySequencePosition<T> left, MemorySequencePosition<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(MemorySequencePosition<T> left, MemorySequencePosition<T> right)
		{
			return !(left==right);
		}
	}
}