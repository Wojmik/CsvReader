using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellStringDeserializer : CellDeserializerFromStringBase<string?>
	{
		private readonly bool EmptyAsNull;

		public CellStringDeserializer(bool emptyAsNull)
		{
			EmptyAsNull = emptyAsNull;
		}

		protected override string? DeserializeFromString(string value)
		{
			string? val = null;

			if(!EmptyAsNull || 0<value.Length)
				val = value;

			return val;
		}
	}
}