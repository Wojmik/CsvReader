using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
#if NET5_0_OR_GREATER
	sealed class CellHalfNullableDeserializer : CellDeserializerFromMemoryBase<Half?>
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellHalfNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
		}

		protected override Half? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			Half? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = Half.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
	}
#endif
}