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
	sealed class CellDecimalDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<decimal>
#else
		CellDeserializerFromStringBase<decimal>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly decimal ValueForEmpty;

		public CellDecimalDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, decimal valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override decimal DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			decimal parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = decimal.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override decimal DeserializeFromString(string value)
		{
			decimal parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = decimal.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}