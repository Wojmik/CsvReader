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
	sealed class CellUShortDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<ushort>
#else
		CellDeserializerFromStringBase<ushort>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly ushort ValueForEmpty;

		public CellUShortDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, ushort valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override ushort DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			ushort parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = ushort.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override ushort DeserializeFromString(string value)
		{
			ushort parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = ushort.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}