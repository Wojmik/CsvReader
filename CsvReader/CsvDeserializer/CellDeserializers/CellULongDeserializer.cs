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
	sealed class CellULongDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<ulong>
#else
		CellDeserializerFromStringBase<ulong>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly ulong _valueForEmpty;

		public CellULongDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, ulong valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override ulong DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			ulong parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = ulong.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override ulong DeserializeFromString(string value)
		{
			ulong parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = ulong.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}