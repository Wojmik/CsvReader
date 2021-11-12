using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellDateOnlyNullableDeserializer : CellDeserializerFromMemoryBase<DateOnly?>
{
	private readonly IFormatProvider FormatProvider;

	private readonly DateTimeStyles DateTimeStyles;

	public CellDateOnlyNullableDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
	{
		FormatProvider = formatProvider;
		DateTimeStyles = dateTimeStyles;
	}

	protected override DateOnly? DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		DateOnly? parsedValue = default;

		if (!value.IsEmpty)
			parsedValue = DateOnly.Parse(value.Span, FormatProvider, DateTimeStyles);
		return parsedValue;
	}
}
#endif