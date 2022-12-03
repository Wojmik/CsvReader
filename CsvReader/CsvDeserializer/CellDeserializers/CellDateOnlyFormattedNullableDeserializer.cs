using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellDateOnlyFormattedNullableDeserializer : CellDeserializerFromMemoryBase<DateOnly?>
{
	private readonly string _format;

	private readonly IFormatProvider _formatProvider;

	private readonly DateTimeStyles _dateTimeStyles;

	public CellDateOnlyFormattedNullableDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
	{
		_format = format;
		_formatProvider = formatProvider;
		_dateTimeStyles = dateTimeStyles;
	}

	protected override DateOnly? DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		DateOnly? parsedValue = default;

		if (!value.IsEmpty)
			parsedValue = DateOnly.ParseExact(value.Span, _format, _formatProvider, _dateTimeStyles);
		return parsedValue;
	}
}
#endif