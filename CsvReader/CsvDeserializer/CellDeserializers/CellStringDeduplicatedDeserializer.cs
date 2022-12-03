using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.Helpers;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellStringDeduplicatedDeserializer : CellDeserializerFromStringBase<string?>
	{
		private readonly bool _emptyAsNull;

		private readonly StringDeduplicator _stringDeduplicator;

		public CellStringDeduplicatedDeserializer(bool emptyAsNull, StringDeduplicator stringDeduplicator)
		{
			_emptyAsNull = emptyAsNull;
			_stringDeduplicator = stringDeduplicator;
		}

		protected override string? DeserializeFromString(string value)
		{
			string? deduplicated = null;

			if(!_emptyAsNull || 0<value.Length)
				deduplicated = _stringDeduplicator.Deduplicate(value);

			return deduplicated;
		}
	}
}