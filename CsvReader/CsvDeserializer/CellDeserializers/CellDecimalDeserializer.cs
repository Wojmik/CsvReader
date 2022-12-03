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
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly decimal _valueForEmpty;

		public CellDecimalDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, decimal valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override decimal DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			decimal parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = decimal.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override decimal DeserializeFromString(string value)
		{
			decimal parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = decimal.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}