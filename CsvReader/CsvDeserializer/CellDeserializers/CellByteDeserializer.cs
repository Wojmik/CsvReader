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
	sealed class CellByteDeserializer :
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<byte>
#else
		CellDeserializerFromStringBase<byte>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly byte ValueForEmpty;

		public CellByteDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, byte valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override byte DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			byte parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = byte.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override byte DeserializeFromString(string value)
		{
			byte parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = byte.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}