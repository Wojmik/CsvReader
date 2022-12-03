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
		private readonly IFormatProvider _formatProvider;

		private readonly DateTimeStyles _dateTimeStyles;

		private readonly bool _allowEmpty;

		private readonly DateTime _valueForEmpty;

		public CellDateTimeDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateTime valueForEmpty)
		{
			_formatProvider = formatProvider;
			_dateTimeStyles = dateTimeStyles;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override DateTime DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			DateTime parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = DateTime.Parse(value.Span, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#else
		protected override DateTime DeserializeFromString(string value)
		{
			DateTime parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = DateTime.Parse(value, _formatProvider, _dateTimeStyles);
			return parsedValue;
		}
#endif
	}
}