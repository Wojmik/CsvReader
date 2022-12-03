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
	sealed class CellSByteDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<sbyte>
#else
		CellDeserializerFromStringBase<sbyte>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly sbyte _valueForEmpty;

		public CellSByteDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, sbyte valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override sbyte DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			sbyte parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = sbyte.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override sbyte DeserializeFromString(string value)
		{
			sbyte parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = sbyte.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}