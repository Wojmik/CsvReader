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
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<DateTimeOffset?>
#else
		CellDeserializerFromStringBase<DateTimeOffset?>
#endif
	{
		private readonly IFormatProvider FormatProvider;

		private readonly DateTimeStyles DateTimeStyles;

		public CellDateTimeOffsetNullableDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
		{
			FormatProvider = formatProvider;
			DateTimeStyles = dateTimeStyles;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override DateTimeOffset? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTimeOffset? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = DateTimeOffset.Parse(value.Span, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTimeOffset? DeserializeFromString(string value)
		{
			DateTimeOffset? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = DateTimeOffset.Parse(value, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#endif
	}
}