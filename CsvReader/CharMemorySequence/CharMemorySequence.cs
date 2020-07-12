using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CharMemorySequence
{
	struct CharMemorySequence
	{
		public CharMemorySequenceSegment First;

		public CharMemorySequenceSegment Last;

		public int StartInFirst;

		public int CountInLast;

		public int Count;

		public void AddNewSegment(int minimumLength)
		{
			this.Last=new CharMemorySequenceSegment(previous: this.Last, minimumLength: minimumLength);
		}

		public void FlipFirstToLast()
		{
			var originalFirst=this.First;
			this.First=originalFirst.Next;
			originalFirst.Reuse(previous: this.Last);
			this.Last=originalFirst;
		}
	}
}