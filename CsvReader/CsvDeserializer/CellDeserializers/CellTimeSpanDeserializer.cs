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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<TimeSpan>
#else
		CellDeserializerFromStringBase<TimeSpan>
#endif
	{
		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly TimeSpan _valueForEmpty;

		public CellTimeSpanDeserializer(IFormatProvider formatProvider, bool allowEmpty, TimeSpan valueForEmpty)
		{
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override TimeSpan DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			TimeSpan parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = TimeSpan.Parse(value.Span, _formatProvider);
			return parsedValue;
		}
#else
		protected override TimeSpan DeserializeFromString(string value)
		{
			TimeSpan parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = TimeSpan.Parse(value, _formatProvider);
			return parsedValue;
		}
#endif
	}
}