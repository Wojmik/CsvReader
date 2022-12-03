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
		private readonly string _format;

		private readonly IFormatProvider _formatProvider;

		private readonly DateTimeStyles _dateTimeStyles;

		public CellDateTimeFormattedNullableDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
		{
			_format = format;
			_formatProvider = formatProvider;
			_dateTimeStyles = dateTimeStyles;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTime? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTime? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = DateTime.ParseExact(value.Span, _format, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTime? DeserializeFromString(string value)
		{
			DateTime? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = DateTime.ParseExact(value, _format, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#endif
	}
}