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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<float?>
#else
		CellDeserializerFromStringBase<float?>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		public CellFloatNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override float? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			float? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = float.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override float? DeserializeFromString(string value)
		{
			float? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = float.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}