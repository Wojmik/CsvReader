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
	sealed class CellDateTimeOffsetDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<DateTimeOffset>
#else
		CellDeserializerFromStringBase<DateTimeOffset>
#endif
	{
		private readonly IFormatProvider FormatProvider;

		private readonly DateTimeStyles DateTimeStyles;

		private readonly bool AllowEmpty;

		private readonly DateTimeOffset ValueForEmpty;

		public CellDateTimeOffsetDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateTimeOffset valueForEmpty)
		{
			FormatProvider = formatProvider;
			DateTimeStyles = dateTimeStyles;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTimeOffset DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTimeOffset parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = DateTimeOffset.Parse(value.Span, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTimeOffset DeserializeFromString(string value)
		{
			DateTimeOffset parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = DateTimeOffset.Parse(value, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#endif
	}
}