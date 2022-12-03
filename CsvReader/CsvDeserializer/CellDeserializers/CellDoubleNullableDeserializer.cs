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
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		public CellDoubleNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override double? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			double? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = double.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override double? DeserializeFromString(string value)
		{
			double? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = double.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}