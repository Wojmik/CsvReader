using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellDateOnlyDeserializer : CellDeserializerFromMemoryBase<DateOnly>
{
	private readonly IFormatProvider _formatProvider;

	private readonly DateTimeStyles _dateTimeStyles;

	private readonly bool _allowEmpty;

	private readonly DateOnly _valueForEmpty;

	public CellDateOnlyDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateOnly valueForEmpty)
	{
		_formatProvider = formatProvider;
		_dateTimeStyles = dateTimeStyles;
		_allowEmpty = allowEmpty;
		_valueForEmpty = valueForEmpty;
	}

	protected override DateOnly DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		DateOnly parsedValue;

		if (_allowEmpty && value.IsEmpty)
			parsedValue = _valueForEmpty;
		else
			parsedValue = DateOnly.Parse(value.Span, _formatProvider, _dateTimeStyles);
		return parsedValue;
	}
}
#endif