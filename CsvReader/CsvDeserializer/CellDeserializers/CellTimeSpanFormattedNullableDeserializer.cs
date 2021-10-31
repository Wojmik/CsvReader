using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellTimeSpanFormattedNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<TimeSpan?>
#else
		CellDeserializerFromStringBase<TimeSpan?>
#endif
	{
		private readonly string Format;

		private readonly IFormatProvider FormatProvider;

		private readonly TimeSpanStyles TimeSpanStyles;

		public CellTimeSpanFormattedNullableDeserializer(string format, IFormatProvider formatProvider, TimeSpanStyles timeSpanStyles)
		{
			Format = format;
			FormatProvider = formatProvider;
			TimeSpanStyles = timeSpanStyles;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override TimeSpan? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			TimeSpan? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = TimeSpan.ParseExact(value.Span, Format, FormatProvider, TimeSpanStyles);
			return parsedValue;
		}
#else
		protected override TimeSpan? DeserializeFromString(string value)
		{
			TimeSpan? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = TimeSpan.ParseExact(value, Format, FormatProvider, TimeSpanStyles);
			return parsedValue;
		}
#endif
	}
}