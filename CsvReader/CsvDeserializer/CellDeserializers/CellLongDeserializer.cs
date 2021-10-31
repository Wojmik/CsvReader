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
	sealed class CellLongDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<long>
#else
		CellDeserializerFromStringBase<long>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly long ValueForEmpty;

		public CellLongDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, long valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override long DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			long parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = long.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override long DeserializeFromString(string value)
		{
			long parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = long.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}