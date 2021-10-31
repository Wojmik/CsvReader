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
	sealed class CellDecimalNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<decimal?>
#else
		CellDeserializerFromStringBase<decimal?>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellDecimalNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override decimal? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			decimal? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = decimal.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override decimal? DeserializeFromString(string value)
		{
			decimal? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = decimal.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}