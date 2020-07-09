using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReader
{
	public class CsvReader
	{
		private TextReader TextReader { get; }

		public char Delimiter { get; }
		public char EscapeChar { get; }

		private readonly System.Numerics.Vector<ushort> SearchVector;

		public CsvReader(TextReader textReader)
		{
			if(textReader==null)
				throw new ArgumentNullException(nameof(textReader));

			this.TextReader=textReader;

			//Create data for SearchVector
			ushort[] searchVactorData = new ushort[System.Numerics.Vector<ushort>.Count];
			searchVactorData[0]=Delimiter;
			searchVactorData[1]=EscapeChar;
			searchVactorData[2]='\r';
			searchVactorData[3]='\n';
			for(int i=4; i<searchVactorData.Length; i++)
				searchVactorData[i]=Delimiter;

			this.SearchVector=new System.Numerics.Vector<ushort>(searchVactorData);
		}

		public async ValueTask<CharArrayNode> ReadNextCharArrayNodeAsync()
		{

		}

		private async ValueTask<long> FindKeyChar()
		{

		}
	}

	public class DataChunk<T> : ReadOnlySequenceSegment<T>
	{
		public DataChunk()
		{
			System.Numerics.Vector<ushort> vector;
			System.Numerics.Vector.

			new System.IO.StreamReader("")
		}
	}
}