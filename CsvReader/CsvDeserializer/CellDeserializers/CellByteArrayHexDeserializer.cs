﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellByteArrayHexDeserializer : CellDeserializerFromMemoryBase<byte[]?>
	{
		private readonly bool EmptyAsNull;

		public CellByteArrayHexDeserializer(bool emptyAsNull)
		{
			EmptyAsNull = emptyAsNull;
		}

		protected override byte[]? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			byte[]? val = null;

			if(!EmptyAsNull || 0<value.Length)
			{
				if((value.Length&1)==1)
					throw new ArgumentException("Broken hex string");

				val = new byte[value.Length/2];
				for(int i = 0; i<val.Length; i++)
				{
#if NETSTANDARD2_1_OR_GREATER
					val[i] = byte.Parse(value.Span.Slice(i*2, 2), style: NumberStyles.HexNumber);
#else
					val[i] = byte.Parse(value.Slice(i*2, 2).ToString(), NumberStyles.HexNumber);
#endif
				}
			}

			return val;
		}
	}
}