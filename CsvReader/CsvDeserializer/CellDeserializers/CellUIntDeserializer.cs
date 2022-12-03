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
	sealed class CellUIntDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<uint>
#else
		CellDeserializerFromStringBase<uint>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly uint _valueForEmpty;

		public CellUIntDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, uint valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override uint DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			uint parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = uint.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override uint DeserializeFromString(string value)
		{
			uint parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = uint.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}