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
	sealed class CellIntNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<int?>
#else
		CellDeserializerFromStringBase<int?>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellIntNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override int? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			int? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = int.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override int? DeserializeFromString(string value)
		{
			int? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = int.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}