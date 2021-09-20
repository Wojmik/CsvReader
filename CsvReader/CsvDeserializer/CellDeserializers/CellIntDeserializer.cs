using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellIntDeserializer :
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<int>
#else
		CellDeserializerFromStringBase<int>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		public CellIntDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider)
		{
			this.NumberStyles = numberStyles;
			this.FormatProvider = formatProvider;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override int DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			var parsedValue = int.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override int DeserializeFromString(string value)
		{
			var parsedValue = int.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}