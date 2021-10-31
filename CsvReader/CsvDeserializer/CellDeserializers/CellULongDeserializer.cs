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
	sealed class CellULongDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<ulong>
#else
		CellDeserializerFromStringBase<ulong>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly ulong ValueForEmpty;

		public CellULongDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, ulong valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override ulong DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			ulong parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = ulong.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override ulong DeserializeFromString(string value)
		{
			ulong parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = ulong.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}