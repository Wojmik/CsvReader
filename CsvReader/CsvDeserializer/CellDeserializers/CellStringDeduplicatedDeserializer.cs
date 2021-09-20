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
		private readonly bool EmptyAsNull;

		private readonly StringDeduplicator StringDeduplicator;

		public CellStringDeduplicatedDeserializer(bool emptyAsNull, StringDeduplicator stringDeduplicator)
		{
			EmptyAsNull = emptyAsNull;
			StringDeduplicator = stringDeduplicator;
		}

		protected override string? DeserializeFromString(string value)
		{
			string? deduplicated = null;

			if(!EmptyAsNull || 0<value.Length)
				deduplicated = StringDeduplicator.Deduplicate(value);

			return deduplicated;
		}
	}
}