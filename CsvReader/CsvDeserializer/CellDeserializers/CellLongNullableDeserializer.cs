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
	sealed class CellLongNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<long?>
#else
		CellDeserializerFromStringBase<long?>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellLongNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override long? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			long? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = long.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override long? DeserializeFromString(string value)
		{
			long? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = long.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}