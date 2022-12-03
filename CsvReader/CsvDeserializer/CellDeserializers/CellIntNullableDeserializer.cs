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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<int?>
#else
		CellDeserializerFromStringBase<int?>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		public CellIntNullableDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override int? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			int? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = int.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override int? DeserializeFromString(string value)
		{
			int? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = int.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}