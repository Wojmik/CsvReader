using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellDateTimeOffsetFormattedNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<DateTimeOffset?>
#else
		CellDeserializerFromStringBase<DateTimeOffset?>
#endif
	{
		private readonly string Format;

		private readonly IFormatProvider FormatProvider;

		private readonly DateTimeStyles DateTimeStyles;

		public CellDateTimeOffsetFormattedNullableDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
		{
			Format = format;
			FormatProvider = formatProvider;
			DateTimeStyles = dateTimeStyles;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTimeOffset? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTimeOffset? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = DateTimeOffset.ParseExact(value.Span, Format, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTimeOffset? DeserializeFromString(string value)
		{
			DateTimeOffset? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = DateTimeOffset.ParseExact(value, Format, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#endif
	}
}