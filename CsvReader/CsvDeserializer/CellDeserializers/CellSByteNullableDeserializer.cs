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
	sealed class CellSByteNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<sbyte?>
#else
		CellDeserializerFromStringBase<sbyte?>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellSByteNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override sbyte? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			sbyte? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = sbyte.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override sbyte? DeserializeFromString(string value)
		{
			sbyte? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = sbyte.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}