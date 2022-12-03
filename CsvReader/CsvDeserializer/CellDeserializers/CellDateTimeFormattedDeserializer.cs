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
		private readonly string _format;

		private readonly IFormatProvider _formatProvider;

		private readonly DateTimeStyles _dateTimeStyles;

		private readonly bool _allowEmpty;

		private readonly DateTime _valueForEmpty;

		public CellDateTimeFormattedDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateTime valueForEmpty)
		{
			_format = format;
			_formatProvider = formatProvider;
			_dateTimeStyles = dateTimeStyles;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTime DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTime parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = DateTime.ParseExact(value.Span, _format, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTime DeserializeFromString(string value)
		{
			DateTime parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = DateTime.ParseExact(value, _format, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#endif
	}
}