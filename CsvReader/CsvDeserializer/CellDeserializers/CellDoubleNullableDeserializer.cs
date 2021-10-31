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
	sealed class CellDoubleNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<double?>
#else
		CellDeserializerFromStringBase<double?>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellDoubleNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override double? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			double? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = double.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override double? DeserializeFromString(string value)
		{
			double? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = double.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}