using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.MemorySequence
{
	struct MemorySequence<T>
	{
		public MemorySequencePosition<T> CurrentPosition { get; private set; }

		public MemorySequenceSegment<T> LastSegment { get; private set; }

		public void AddNewSegment(int minimumLength)
		{
			this.LastSegment=new MemorySequenceSegment<T>(previous: this.LastSegment, minimumLength: minimumLength);
			
			//If it is the first segment - set FirstSegment and CurrentPosition
			if(this.CurrentPosition.InternalSequenceSegment==null)
				this.CurrentPosition=new MemorySequencePosition<T>(sequenceSegment: this.LastSegment, positionInSegment: 0);
		}

		public void MoveForward(in MemorySequencePosition<T> newPosition)
		{
			var currentSegment = this.CurrentPosition.InternalSequenceSegment;

			while(!object.ReferenceEquals(currentSegment, newPosition.InternalSequenceSegment))
			{
				//Flip current segment to last
				currentSegment=FlipSegmentToLast(segmentToFlip: currentSegment);

				//Infinite loop detection
				if(object.ReferenceEquals(this.CurrentPosition, currentSegment))
				{
					//Full rotation so paradoxically state of MemorySequence did not change
					throw new ArgumentException($"{nameof(newPosition)} doesn't belong to this {nameof(MemorySequence<T>)}", nameof(newPosition));
				}
			}

			this.CurrentPosition=newPosition;
		}

		private MemorySequenceSegment<T> FlipSegmentToLast(MemorySequenceSegment<T> segmentToFlip)
		{
			var newCurrent = segmentToFlip.NextInternal;

			segmentToFlip.Reuse(previous: this.LastSegment);
			this.LastSegment=segmentToFlip;

			return newCurrent;
		}
	}
}