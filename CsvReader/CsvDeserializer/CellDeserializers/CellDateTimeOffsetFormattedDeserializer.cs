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
		private readonly string _format;

		private readonly IFormatProvider _formatProvider;

		private readonly DateTimeStyles _dateTimeStyles;

		private readonly bool _allowEmpty;

		private readonly DateTimeOffset _valueForEmpty;

		public CellDateTimeOffsetFormattedDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateTimeOffset valueForEmpty)
		{
			_format = format;
			_formatProvider = formatProvider;
			_dateTimeStyles = dateTimeStyles;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTimeOffset DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTimeOffset parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = DateTimeOffset.ParseExact(value.Span, _format, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTimeOffset DeserializeFromString(string value)
		{
			DateTimeOffset parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = DateTimeOffset.ParseExact(value, _format, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#endif
	}
}