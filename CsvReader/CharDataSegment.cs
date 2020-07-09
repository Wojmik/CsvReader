using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader
{
	class CharDataSegment : IDisposable
	{
		public char[] Segment { get; private set; }

		public int Start { get; set; }

		public int Count { get; set; }

		public CharDataSegment Next { get; private set; }

		public CharDataSegment(CharDataSegment previous, int minimumLength)
		{
			if(previous!=null)
				previous.Next=this;

			Segment=ArrayPool<char>.Shared.Rent(minimumLength);
		}

		public void Reuse(CharDataSegment previous)
		{
			this.Next=null;
			this.Start=0;
			this.Count=0;

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