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
	sealed class CellIntDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<int>
#else
		CellDeserializerFromStringBase<int>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly int _valueForEmpty;

		public CellIntDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, int valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override int DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			int parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = int.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override int DeserializeFromString(string value)
		{
			int parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = int.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}