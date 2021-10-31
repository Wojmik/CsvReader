using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellTimeSpanFormattedDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<TimeSpan>
#else
		CellDeserializerFromStringBase<TimeSpan>
#endif
	{
		private readonly string Format;

		private readonly IFormatProvider FormatProvider;

		private readonly TimeSpanStyles TimeSpanStyles;

		private readonly bool AllowEmpty;

		private readonly TimeSpan ValueForEmpty;

		public CellTimeSpanFormattedDeserializer(string format, IFormatProvider formatProvider, TimeSpanStyles timeSpanStyles, bool allowEmpty, TimeSpan valueForEmpty)
		{
			Format = format;
			FormatProvider = formatProvider;
			TimeSpanStyles = timeSpanStyles;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override TimeSpan DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			TimeSpan parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = TimeSpan.ParseExact(value.Span, Format, FormatProvider, TimeSpanStyles);
			return parsedValue;
		}
#else
		protected override TimeSpan DeserializeFromString(string value)
		{
			TimeSpan parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = TimeSpan.ParseExact(value, Format, FormatProvider, TimeSpanStyles);
			return parsedValue;
		}
#endif
	}
}