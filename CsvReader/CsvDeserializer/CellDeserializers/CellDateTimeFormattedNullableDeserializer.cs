using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellDateTimeFormattedNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<DateTime?>
#else
		CellDeserializerFromStringBase<DateTime?>
#endif
	{
		private readonly string Format;

		private readonly IFormatProvider FormatProvider;

		private readonly DateTimeStyles DateTimeStyles;

		public CellDateTimeFormattedNullableDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
		{
			Format = format;
			FormatProvider = formatProvider;
			DateTimeStyles = dateTimeStyles;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTime? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTime? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = DateTime.ParseExact(value.Span, Format, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTime? DeserializeFromString(string value)
		{
			DateTime? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = DateTime.ParseExact(value, Format, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#endif
	}
}