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
		private readonly string _format;

		private readonly IFormatProvider _formatProvider;

		private readonly TimeSpanStyles _timeSpanStyles;

		private readonly bool _allowEmpty;

		private readonly TimeSpan _valueForEmpty;

		public CellTimeSpanFormattedDeserializer(string format, IFormatProvider formatProvider, TimeSpanStyles timeSpanStyles, bool allowEmpty, TimeSpan valueForEmpty)
		{
			_format = format;
			_formatProvider = formatProvider;
			_timeSpanStyles = timeSpanStyles;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override TimeSpan DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			TimeSpan parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = TimeSpan.ParseExact(value.Span, _format, _formatProvider, _timeSpanStyles);
			return parsedValue;
		}
#else
		protected override TimeSpan DeserializeFromString(string value)
		{
			TimeSpan parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = TimeSpan.ParseExact(value, _format, _formatProvider, _timeSpanStyles);
			return parsedValue;
		}
#endif
	}
}