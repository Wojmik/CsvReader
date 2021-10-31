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
	sealed class CellUIntDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<uint>
#else
		CellDeserializerFromStringBase<uint>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly uint ValueForEmpty;

		public CellUIntDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, uint valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override uint DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			uint parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = uint.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override uint DeserializeFromString(string value)
		{
			uint parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = uint.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}