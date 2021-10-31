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
	sealed class CellTimeSpanNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<TimeSpan?>
#else
		CellDeserializerFromStringBase<TimeSpan?>
#endif
	{
		private readonly IFormatProvider FormatProvider;

		public CellTimeSpanNullableDeserializer(IFormatProvider formatProvider)
		{
			FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override TimeSpan? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			TimeSpan? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = TimeSpan.Parse(value.Span, FormatProvider);
			return parsedValue;
		}
#else
		protected override TimeSpan? DeserializeFromString(string value)
		{
			TimeSpan? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = TimeSpan.Parse(value, FormatProvider);
			return parsedValue;
		}
#endif
	}
}