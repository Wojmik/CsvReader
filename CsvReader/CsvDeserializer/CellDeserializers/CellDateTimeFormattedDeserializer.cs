using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellDateTimeFormattedDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<DateTime>
#else
		CellDeserializerFromStringBase<DateTime>
#endif
	{
		private readonly string Format;

		private readonly IFormatProvider FormatProvider;

		private readonly DateTimeStyles DateTimeStyles;

		private readonly bool AllowEmpty;

		private readonly DateTime ValueForEmpty;

		public CellDateTimeFormattedDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateTime valueForEmpty)
		{
			Format = format;
			FormatProvider = formatProvider;
			DateTimeStyles = dateTimeStyles;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTime DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTime parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = DateTime.ParseExact(value.Span, Format, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTime DeserializeFromString(string value)
		{
			DateTime parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = DateTime.ParseExact(value, Format, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#endif
	}
}