using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
#if NET5_0_OR_GREATER
	sealed class CellHalfDeserializer : CellDeserializerFromMemoryBase<Half>
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly Half _valueForEmpty;

		public CellHalfDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, Half valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

		protected override Half DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			Half parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = Half.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
	}
#endif
}