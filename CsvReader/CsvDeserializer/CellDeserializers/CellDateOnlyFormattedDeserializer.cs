using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellDateOnlyFormattedDeserializer : CellDeserializerFromMemoryBase<DateOnly>
{
	private readonly string _format;

	private readonly IFormatProvider _formatProvider;

	private readonly DateTimeStyles _dateTimeStyles;

	private readonly bool _allowEmpty;

	private readonly DateOnly _valueForEmpty;

	public CellDateOnlyFormattedDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateOnly valueForEmpty)
	{
		_format = format;
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
			parsedValue = DateOnly.ParseExact(value.Span, _format, _formatProvider, _dateTimeStyles);
		return parsedValue;
	}
}
#endif