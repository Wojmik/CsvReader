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
	sealed class CellLongDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<long>
#else
		CellDeserializerFromStringBase<long>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly long _valueForEmpty;

		public CellLongDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, long valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override long DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			long parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = long.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override long DeserializeFromString(string value)
		{
			long parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = long.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}