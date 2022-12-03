using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellDateTimeOffsetNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<DateTimeOffset?>
#else
		CellDeserializerFromStringBase<DateTimeOffset?>
#endif
	{
		private readonly IFormatProvider _formatProvider;

		private readonly DateTimeStyles _dateTimeStyles;

		public CellDateTimeOffsetNullableDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
		{
			_formatProvider = formatProvider;
			_dateTimeStyles = dateTimeStyles;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTimeOffset? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTimeOffset? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = DateTimeOffset.Parse(value.Span, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTimeOffset? DeserializeFromString(string value)
		{
			DateTimeOffset? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = DateTimeOffset.Parse(value, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#endif
	}
}