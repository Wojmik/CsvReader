using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellDateOnlyDeserializer : CellDeserializerFromMemoryBase<DateOnly>
{
	private readonly IFormatProvider FormatProvider;

	private readonly DateTimeStyles DateTimeStyles;

	private readonly bool AllowEmpty;

	private readonly DateOnly ValueForEmpty;

	public CellDateOnlyDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, DateOnly valueForEmpty)
	{
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
			parsedValue = DateOnly.Parse(value.Span, FormatProvider, DateTimeStyles);
		return parsedValue;
	}
}
#endif