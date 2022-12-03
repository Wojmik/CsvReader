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
	sealed class CellDateTimeNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<DateTime?>
#else
		CellDeserializerFromStringBase<DateTime?>
#endif
	{
		private readonly IFormatProvider _formatProvider;

		private readonly DateTimeStyles _dateTimeStyles;

		public CellDateTimeNullableDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
		{
			_formatProvider = formatProvider;
			_dateTimeStyles = dateTimeStyles;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTime? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTime? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = DateTime.Parse(value.Span, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTime? DeserializeFromString(string value)
		{
			DateTime? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = DateTime.Parse(value, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#endif
	}
}