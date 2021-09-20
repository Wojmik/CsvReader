using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.MemorySequence
{
	/// <summary>
	/// Memory sequence
	/// </summary>
	/// <typeparam name="T">Type of items</typeparam>
	struct MemorySequence<T> : IDisposable
	{
		/// <summary>
		/// Current position
		/// </summary>
		public MemorySequencePosition<T> CurrentPosition { get; private set; }

		/// <summary>
		/// Last segment - most ahead segment
		/// </summary>
		public MemorySequenceSegment<T> LastSegment { get; private set; }

		public void AddNewSegment(int minimumLength)
		{
			this.LastSegment=new MemorySequenceSegment<T>(previous: this.LastSegment, minimumLength: minimumLength);
			
			//If it is the first segment - set FirstSegment and CurrentPosition
			if(this.CurrentPosition.InternalSequenceSegment==null)
				this.CurrentPosition=new MemorySequencePosition<T>(sequenceSegment: this.LastSegment, positionInSegment: 0);
		}

		/// <summary>
		/// Moves <see cref="CurrentPosition"/> to <paramref name="newPosition"/> plus <paramref name="offset"/>
		/// </summary>
		/// <param name="newPosition">New position</param>
		/// <param name="offset">Offset above <paramref name="newPosition"/></param>
		public void MoveForward(in MemorySequencePosition<T> newPosition, int offset)
		{
			var currentSegment = this.CurrentPosition.InternalSequenceSegment;
			int newPositionInSegment = newPosition.PositionInSegment + offset;

			Debug.Assert(offset>=0, $"{nameof(offset)} cannot be negative");

			while(!object.ReferenceEquals(currentSegment, newPosition.InternalSequenceSegment))
			{
				if(currentSegment.Memory.Length<currentSegment.Array.Length)
					throw new InvalidOperationException("Cannot move above filled area");//Cannot flip not full segment

				//Flip current segment to last
				currentSegment=FlipSegmentToLast(segmentToFlip: currentSegment);

				//Infinite loop detection
				if(object.ReferenceEquals(this.CurrentPosition.InternalSequenceSegment, currentSegment))
				{
					//Full rotation so paradoxically state of MemorySequence did not change
					throw new ArgumentException($"{nameof(newPosition)} doesn't belong to this {nameof(MemorySequence<T>)}", nameof(newPosition));
				}
			}

			while(currentSegment.Array.Length<=newPositionInSegment)
			{
				if(currentSegment.Memory.Length<currentSegment.Array.Length)
					throw new InvalidOperationException("Cannot move above filled area");//Cannot flip not full segment

				newPositionInSegment -= currentSegment.Array.Length;

				//Flip current segment to last
				currentSegment=FlipSegmentToLast(segmentToFlip: currentSegment);
			}

			if(currentSegment.Memory.Length<newPositionInSegment)
				throw new InvalidOperationException("Cannot move above filled area");

			this.CurrentPosition = new MemorySequencePosition<T>(currentSegment, newPositionInSegment);
		}

		private MemorySequenceSegment<T> FlipSegmentToLast(MemorySequenceSegment<T> segmentToFlip)
		{
			var newCurrent = segmentToFlip.NextInternal;

			segmentToFlip.Reuse(previous: this.LastSegment);
			this.LastSegment=segmentToFlip;

			return newCurrent!;
		}

		public void Dispose()
		{
			var segment = CurrentPosition.InternalSequenceSegment;
			while(segment!=null)
			{
				segment.Dispose();
				segment = segment.NextInternal;
			}
		}
	}
}