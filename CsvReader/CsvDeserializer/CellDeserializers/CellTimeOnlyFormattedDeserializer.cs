using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellTimeOnlyFormattedDeserializer : CellDeserializerFromMemoryBase<TimeOnly>
{
	private readonly string _format;

	private readonly IFormatProvider _formatProvider;

	private readonly DateTimeStyles _dateTimeStyles;

	private readonly bool _allowEmpty;

	private readonly TimeOnly _valueForEmpty;

	public CellTimeOnlyFormattedDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, TimeOnly valueForEmpty)
	{
		_format = format;
		_formatProvider = formatProvider;
		_dateTimeStyles = dateTimeStyles;
		_allowEmpty = allowEmpty;
		_valueForEmpty = valueForEmpty;
	}

	protected override TimeOnly DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		TimeOnly parsedValue;

		if (_allowEmpty && value.IsEmpty)
			parsedValue = _valueForEmpty;
		else
			parsedValue = TimeOnly.ParseExact(value.Span, _format, _formatProvider, _dateTimeStyles);
		return parsedValue;
	}
}
#endif