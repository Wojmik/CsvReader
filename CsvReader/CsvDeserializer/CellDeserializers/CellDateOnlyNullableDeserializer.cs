using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellDateOnlyNullableDeserializer : CellDeserializerFromMemoryBase<DateOnly?>
{
	private readonly IFormatProvider _formatProvider;

	private readonly DateTimeStyles _dateTimeStyles;

	public CellDateOnlyNullableDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
	{
		_formatProvider = formatProvider;
		_dateTimeStyles = dateTimeStyles;
	}

	protected override DateOnly? DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		DateOnly? parsedValue = default;

		if (!value.IsEmpty)
			parsedValue = DateOnly.Parse(value.Span, _formatProvider, _dateTimeStyles);
		return parsedValue;
	}
}
#endif