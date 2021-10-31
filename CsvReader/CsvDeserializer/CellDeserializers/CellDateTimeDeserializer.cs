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
	sealed class CellDateTimeDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<DateTime>
#else
		CellDeserializerFromStringBase<DateTime>
#endif
	{
		private readonly IFormatProvider FormatProvider;

		private readonly DateTimeStyles DateTimeStyles;

		private readonly bool AllowEmpty;

		private readonly DateTime ValueForEmpty;

		public CellDateTimeDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateTime valueForEmpty)
		{
			FormatProvider = formatProvider;
			DateTimeStyles = dateTimeStyles;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTime DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTime parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = DateTime.Parse(value.Span, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTime DeserializeFromString(string value)
		{
			DateTime parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = DateTime.Parse(value, FormatProvider, DateTimeStyles);
			return parsedValue;
		}
#endif
	}
}