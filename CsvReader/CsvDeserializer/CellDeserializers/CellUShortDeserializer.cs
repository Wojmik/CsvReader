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
	sealed class CellUShortDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<ushort>
#else
		CellDeserializerFromStringBase<ushort>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly ushort _valueForEmpty;

		public CellUShortDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, ushort valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override ushort DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			ushort parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = ushort.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override ushort DeserializeFromString(string value)
		{
			ushort parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = ushort.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}