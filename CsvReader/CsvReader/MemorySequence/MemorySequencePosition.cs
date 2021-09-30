using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.MemorySequence
{
	public readonly struct MemorySequencePosition<T> : IEquatable<MemorySequencePosition<T>>
	{
		internal readonly MemorySequenceSegment<T> InternalSequenceSegment;

		public ReadOnlySequenceSegment<T> SequenceSegment { get => InternalSequenceSegment; }

		public readonly int PositionInSegment;

		public long AbsolutePosition { get => this.SequenceSegment.RunningIndex+this.PositionInSegment; }

		internal MemorySequencePosition(MemorySequenceSegment<T> sequenceSegment, int positionInSegment)
		{
			this.InternalSequenceSegment=sequenceSegment??throw new ArgumentNullException(nameof(sequenceSegment));
			this.PositionInSegment=positionInSegment;
		}

		public MemorySequencePosition<T> AddOffset(int offset)
		{
			var currentSegment = InternalSequenceSegment;
			int newPositionInSegment = PositionInSegment + offset;

			if(offset<0)
				throw new ArgumentOutOfRangeException(nameof(offset), offset, $"{nameof(offset)} cannot be negative");

			while(currentSegment.Array.Length<=newPositionInSegment)
			{
				if(currentSegment.Memory.Length<currentSegment.Array.Length)
					throw new ArgumentOutOfRangeException(nameof(offset), offset, "Cannot move above filled area");//Cannot flip not full segment

				newPositionInSegment -= currentSegment.Array.Length;

				//Flip current segment to last
				currentSegment=currentSegment.NextInternal;
				if(currentSegment==null)
					throw new ArgumentOutOfRangeException(nameof(offset), offset, $"Cannot move above filled area");
			}

			if(currentSegment.Memory.Length<newPositionInSegment)
				throw new ArgumentOutOfRangeException(nameof(offset), offset, "Cannot move above filled area");

			return new MemorySequencePosition<T>(currentSegment, newPositionInSegment);
		}

		public override bool Equals(object obj)
		{
			return obj is MemorySequencePosition<T> position && Equals(position);
		}

		public bool Equals(MemorySequencePosition<T> other)
		{
			return other!=null
				&& PositionInSegment==other.PositionInSegment
				&& InternalSequenceSegment.Array.Equals(other.InternalSequenceSegment.Array);
				//&& SequenceSegment.Memory.Equals(other.SequenceSegment.Memory);
		}

		public override int GetHashCode()
		{
#if NETSTANDARD2_1_OR_GREATER
			HashCode hashCode = new HashCode();
			hashCode.Add(InternalSequenceSegment.Array);
			//hashCode.Add(SequenceSegment.Memory);
			hashCode.Add(PositionInSegment);
			return hashCode.ToHashCode();
#else
			int hashCode = -1739113237;
			hashCode=hashCode*-1521134295+InternalSequenceSegment.Array.GetHashCode();
			//hashCode=hashCode*-1521134295+SequenceSegment.Memory.GetHashCode();
			hashCode=hashCode*-1521134295+PositionInSegment.GetHashCode();
			return hashCode;
#endif
		}

		public static bool operator ==(MemorySequencePosition<T> left, MemorySequencePosition<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(MemorySequencePosition<T> left, MemorySequencePosition<T> right)
		{
			return !(left==right);
		}

		public static MemorySequencePosition<T> operator +(MemorySequencePosition<T> position, int offset)
		{
			return position.AddOffset(offset);
		}
	}
}