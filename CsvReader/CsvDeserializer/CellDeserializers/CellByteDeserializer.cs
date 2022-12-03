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
	sealed class CellByteDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<byte>
#else
		CellDeserializerFromStringBase<byte>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly byte _valueForEmpty;

		public CellByteDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, byte valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override byte DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			byte parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = byte.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override byte DeserializeFromString(string value)
		{
			byte parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = byte.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}