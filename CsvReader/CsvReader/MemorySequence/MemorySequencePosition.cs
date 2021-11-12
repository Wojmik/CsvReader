using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.MemorySequence
{
	/// <summary>
	/// Position in memory sequence
	/// </summary>
	/// <typeparam name="T">Type of elements in memory sequence</typeparam>
	public readonly struct MemorySequencePosition<T> : IEquatable<MemorySequencePosition<T>>
	{
		internal readonly MemorySequenceSegment<T> InternalSequenceSegment;

		/// <summary>
		/// Memory sequence segment
		/// </summary>
		public ReadOnlySequenceSegment<T> SequenceSegment { get => InternalSequenceSegment; }

		/// <summary>
		/// Position in <see cref="SequenceSegment"/>
		/// </summary>
		public readonly int PositionInSegment;

		/// <summary>
		/// Absolute position in whole memory sequence
		/// </summary>
		public long AbsolutePosition { get => this.SequenceSegment.RunningIndex+this.PositionInSegment; }

		internal MemorySequencePosition(MemorySequenceSegment<T> sequenceSegment, int positionInSegment)
		{
			this.InternalSequenceSegment=sequenceSegment??throw new ArgumentNullException(nameof(sequenceSegment));
			this.PositionInSegment=positionInSegment;
		}

		/// <summary>
		/// Adds offset to current <see cref="MemorySequencePosition{T}"/>
		/// </summary>
		/// <param name="offset">Offset</param>
		/// <returns>New <see cref="MemorySequencePosition{T}"/></returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> is negative or leave outside filled area</exception>
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

		/// <summary>
		/// Check equality of current <see cref="MemorySequencePosition{T}"/> and <paramref name="obj"/>
		/// </summary>
		/// <param name="obj">Object to compare</param>
		/// <returns>True if objects are equal, false otherwise</returns>
		public override bool Equals(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			object? obj)
		{
			return obj is MemorySequencePosition<T> position && Equals(position);
		}

		/// <summary>
		/// Check equality of current <see cref="MemorySequencePosition{T}"/> and <paramref name="other"/>
		/// </summary>
		/// <param name="other"><see cref="MemorySequencePosition{T}"/> to compare</param>
		/// <returns>True if objects are equal, false otherwise</returns>
		public bool Equals(MemorySequencePosition<T> other)
		{
			return PositionInSegment==other.PositionInSegment
				&& InternalSequenceSegment.Array.Equals(other.InternalSequenceSegment.Array);
				//&& SequenceSegment.Memory.Equals(other.SequenceSegment.Memory);
		}

		/// <summary>
		/// Returns has code of the current <see cref="MemorySequencePosition{T}"/>
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode()
		{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
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

		/// <summary>
		/// Equality operator
		/// </summary>
		/// <param name="left">Left</param>
		/// <param name="right">Right</param>
		/// <returns>Are <paramref name="left"/> and <paramref name="right"/> equal</returns>
		public static bool operator ==(MemorySequencePosition<T> left, MemorySequencePosition<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Inequality operator
		/// </summary>
		/// <param name="left">Left</param>
		/// <param name="right">Right</param>
		/// <returns>Are <paramref name="left"/> and <paramref name="right"/> not equal</returns>
		public static bool operator !=(MemorySequencePosition<T> left, MemorySequencePosition<T> right)
		{
			return !(left==right);
		}

		/// <summary>
		/// Add <paramref name="offset"/> to <paramref name="position"/> operator
		/// </summary>
		/// <param name="position">Position</param>
		/// <param name="offset">Offset</param>
		/// <returns>New <see cref="MemorySequencePosition{T}"/></returns>
		public static MemorySequencePosition<T> operator +(MemorySequencePosition<T> position, int offset)
		{
			return position.AddOffset(offset);
		}
	}
}