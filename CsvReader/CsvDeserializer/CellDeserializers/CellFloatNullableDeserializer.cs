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
	sealed class CellFloatNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<float?>
#else
		CellDeserializerFromStringBase<float?>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellFloatNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override float? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			float? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = float.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override float? DeserializeFromString(string value)
		{
			float? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = float.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}