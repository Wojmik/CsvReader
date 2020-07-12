using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReader.CharMemorySequence
{
	class CharMemorySequenceSegment
	{
		public char[] Segment { get; private set; }

		public int Start { get; set; }

		public int Count { get; set; }

		public CharMemorySequenceSegment Next { get; private set; }

		public CharMemorySequenceSegment(CharMemorySequenceSegment previous, int minimumLength)
		{
			if(previous!=null)
				previous.Next=this;

			Segment=ArrayPool<char>.Shared.Rent(minimumLength);
		}

		public void Reuse(CharMemorySequenceSegment previous)
		{
			this.Next=null;
			this.Start=0;
			this.Count=0;
			this.LoadingTask=null;

			previous.Next=this;
		}

		public void Dispose()
		{
			char[] segment = this.Segment;

			if(segment!=null)
			{
				ArrayPool<char>.Shared.Return(segment, true);
				this.Segment=null;
			}
		}
	}
}