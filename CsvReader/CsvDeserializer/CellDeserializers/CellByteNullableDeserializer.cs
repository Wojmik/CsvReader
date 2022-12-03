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
	sealed class CellByteNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<byte?>
#else
		CellDeserializerFromStringBase<byte?>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		public CellByteNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override byte? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			byte? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = byte.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override byte? DeserializeFromString(string value)
		{
			byte? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = byte.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}