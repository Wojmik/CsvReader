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
	sealed class CellShortNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<short?>
#else
		CellDeserializerFromStringBase<short?>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellShortNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override short? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			short? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = short.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override short? DeserializeFromString(string value)
		{
			short? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = short.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}