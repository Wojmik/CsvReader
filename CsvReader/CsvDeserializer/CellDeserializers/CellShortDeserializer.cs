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
	sealed class CellShortDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<short>
#else
		CellDeserializerFromStringBase<short>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly short _valueForEmpty;

		public CellShortDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, short valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override short DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			short parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = short.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override short DeserializeFromString(string value)
		{
			short parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = short.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}