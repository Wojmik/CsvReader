using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
#if NET5_0_OR_GREATER
	sealed class CellHalfNullableDeserializer : CellDeserializerFromMemoryBase<Half?>
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		public CellHalfNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
		}

		protected override Half? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			Half? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = Half.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
	}
#endif
}