using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellDateTimeOffsetFormattedDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<DateTimeOffset>
#else
		CellDeserializerFromStringBase<DateTimeOffset>
#endif
	{
		private readonly string Format;

		private readonly IFormatProvider FormatProvider;

		private readonly DateTimeStyles DateTimeStyles;

		private readonly bool AllowEmpty;

		private readonly DateTimeOffset ValueForEmpty;

		public CellDateTimeOffsetFormattedDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateTimeOffset valueForEmpty)
		{
			Format = format;
			FormatProvider = formatProvider;
			DateTimeStyles = dateTimeStyles;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTimeOffset DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTimeOffset parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = DateTimeOffset.ParseExact(value.Span, Format, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTimeOffset DeserializeFromString(string value)
		{
			DateTimeOffset parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = DateTimeOffset.ParseExact(value, Format, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#endif
	}
}