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
	sealed class CellUIntNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<uint?>
#else
		CellDeserializerFromStringBase<uint?>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellUIntNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override uint? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			uint? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = uint.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override uint? DeserializeFromString(string value)
		{
			uint? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = uint.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}