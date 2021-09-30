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
	sealed class CellTimeSpanDeserializer :
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<TimeSpan>
#else
		CellDeserializerFromStringBase<TimeSpan>
#endif
	{
		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly TimeSpan ValueForEmpty;

		public CellTimeSpanDeserializer(IFormatProvider formatProvider, bool allowEmpty, TimeSpan valueForEmpty)
		{
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override TimeSpan DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			TimeSpan parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = TimeSpan.Parse(value.Span, FormatProvider);
			return parsedValue;
		}
#else
		protected override TimeSpan DeserializeFromString(string value)
		{
			TimeSpan parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = TimeSpan.Parse(value, FormatProvider);
			return parsedValue;
		}
#endif
	}
}