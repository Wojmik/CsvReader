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
	sealed class CellULongNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<ulong?>
#else
		CellDeserializerFromStringBase<ulong?>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		public CellULongNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override ulong? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			ulong? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = ulong.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override ulong? DeserializeFromString(string value)
		{
			ulong? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = ulong.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}