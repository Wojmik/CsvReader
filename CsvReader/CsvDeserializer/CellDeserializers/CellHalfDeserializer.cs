using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
#if NET5_0_OR_GREATER
	sealed class CellHalfDeserializer : CellDeserializerFromMemoryBase<Half>
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly Half ValueForEmpty;

		public CellHalfDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, Half valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

		protected override Half DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			Half parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = Half.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
	}
#endif
}