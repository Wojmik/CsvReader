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
		private readonly string _format;

		private readonly IFormatProvider _formatProvider;

		private readonly TimeSpanStyles _timeSpanStyles;

		public CellTimeSpanFormattedNullableDeserializer(string format, IFormatProvider formatProvider, TimeSpanStyles timeSpanStyles)
		{
			_format = format;
			_formatProvider = formatProvider;
			_timeSpanStyles = timeSpanStyles;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override TimeSpan? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			TimeSpan? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = TimeSpan.ParseExact(value.Span, _format, _formatProvider, _timeSpanStyles);
			return parsedValue;
		}
#else
		protected override TimeSpan? DeserializeFromString(string value)
		{
			TimeSpan? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = TimeSpan.ParseExact(value, _format, _formatProvider, _timeSpanStyles);
			return parsedValue;
		}
#endif
	}
}