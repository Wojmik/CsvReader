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
		private readonly bool _emptyAsNull;

		public CellByteArrayBase64Deserializer(bool emptyAsNull)
		{
			_emptyAsNull = emptyAsNull;
		}

		protected override byte[]? DeserializeFromString(string value)
		{
			byte[]? val = null;

			if(!_emptyAsNull || 0<value.Length)
			{
				val = Convert.FromBase64String(value);
			}

			return val;
		}
	}
}