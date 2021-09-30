using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellByteArrayBase64Deserializer : CellDeserializerFromStringBase<byte[]?>
	{
		private readonly bool EmptyAsNull;

		public CellByteArrayBase64Deserializer(bool emptyAsNull)
		{
			EmptyAsNull = emptyAsNull;
		}

		protected override byte[]? DeserializeFromString(string value)
		{
			byte[]? val = null;

			if(!EmptyAsNull || 0<value.Length)
			{
				val = Convert.FromBase64String(value);
			}

			return val;
		}
	}
}