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
		private readonly string _format;

		private readonly IFormatProvider _formatProvider;

		private readonly DateTimeStyles _dateTimeStyles;

		public CellDateTimeOffsetFormattedNullableDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
		{
			_format = format;
			_formatProvider = formatProvider;
			_dateTimeStyles = dateTimeStyles;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTimeOffset? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTimeOffset? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = DateTimeOffset.ParseExact(value.Span, _format, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTimeOffset? DeserializeFromString(string value)
		{
			DateTimeOffset? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = DateTimeOffset.ParseExact(value, _format, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#endif
	}
}