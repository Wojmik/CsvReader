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
	sealed class CellShortDeserializer :
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<short>
#else
		CellDeserializerFromStringBase<short>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly short ValueForEmpty;

		public CellShortDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, short valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override short DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			short parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = short.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override short DeserializeFromString(string value)
		{
			short parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = short.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}