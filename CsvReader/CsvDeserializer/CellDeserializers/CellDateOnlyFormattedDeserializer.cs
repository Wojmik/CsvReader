using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellDateOnlyFormattedDeserializer : CellDeserializerFromMemoryBase<DateOnly>
{
	private readonly string Format;

	private readonly IFormatProvider FormatProvider;

	private readonly DateTimeStyles DateTimeStyles;

	private readonly bool AllowEmpty;

	private readonly DateOnly ValueForEmpty;

	public CellDateOnlyFormattedDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateOnly valueForEmpty)
	{
		Format = format;
		FormatProvider = formatProvider;
		DateTimeStyles = dateTimeStyles;
		AllowEmpty = allowEmpty;
		ValueForEmpty = valueForEmpty;
	}

	protected override DateOnly DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		DateOnly parsedValue;

		if (AllowEmpty && value.IsEmpty)
			parsedValue = ValueForEmpty;
		else
			parsedValue = DateOnly.ParseExact(value.Span, Format, FormatProvider, DateTimeStyles);
		return parsedValue;
	}
}
#endif