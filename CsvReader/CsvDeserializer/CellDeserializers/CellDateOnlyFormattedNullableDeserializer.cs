using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellDateOnlyFormattedNullableDeserializer : CellDeserializerFromMemoryBase<DateOnly?>
{
	private readonly string Format;

	private readonly IFormatProvider FormatProvider;

	private readonly DateTimeStyles DateTimeStyles;

	public CellDateOnlyFormattedNullableDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
	{
		Format = format;
		FormatProvider = formatProvider;
		DateTimeStyles = dateTimeStyles;
	}

	protected override DateOnly? DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		DateOnly? parsedValue = default;

		if (!value.IsEmpty)
			parsedValue = DateOnly.ParseExact(value.Span, Format, FormatProvider, DateTimeStyles);
		return parsedValue;
	}
}
#endif