using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellStringDeserializer : CellDeserializerFromStringBase<string?>
	{
		private readonly bool _emptyAsNull;

		public CellStringDeserializer(bool emptyAsNull)
		{
			_emptyAsNull = emptyAsNull;
		}

		protected override string? DeserializeFromString(string value)
		{
			string? val = null;

			if(!_emptyAsNull || 0<value.Length)
				val = value;

			return val;
		}
	}
}