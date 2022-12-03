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
	sealed class CellUShortNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<ushort?>
#else
		CellDeserializerFromStringBase<ushort?>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		public CellUShortNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override ushort? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			ushort? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = ushort.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override ushort? DeserializeFromString(string value)
		{
			ushort? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = ushort.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}