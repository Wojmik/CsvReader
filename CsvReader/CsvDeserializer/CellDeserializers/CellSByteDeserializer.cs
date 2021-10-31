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
	sealed class CellSByteDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<sbyte>
#else
		CellDeserializerFromStringBase<sbyte>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly sbyte ValueForEmpty;

		public CellSByteDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, sbyte valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override sbyte DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			sbyte parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = sbyte.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override sbyte DeserializeFromString(string value)
		{
			sbyte parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = sbyte.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}