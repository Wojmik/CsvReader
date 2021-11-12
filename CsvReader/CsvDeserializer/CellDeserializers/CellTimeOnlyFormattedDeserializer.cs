using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellTimeOnlyFormattedDeserializer : CellDeserializerFromMemoryBase<TimeOnly>
{
	private readonly string Format;

	private readonly IFormatProvider FormatProvider;

	private readonly DateTimeStyles DateTimeStyles;

	private readonly bool AllowEmpty;

	private readonly TimeOnly ValueForEmpty;

	public CellTimeOnlyFormattedDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, TimeOnly valueForEmpty)
	{
		Format = format;
		FormatProvider = formatProvider;
		DateTimeStyles = dateTimeStyles;
		AllowEmpty = allowEmpty;
		ValueForEmpty = valueForEmpty;
	}

	protected override TimeOnly DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		TimeOnly parsedValue;

		if (AllowEmpty && value.IsEmpty)
			parsedValue = ValueForEmpty;
		else
			parsedValue = TimeOnly.ParseExact(value.Span, Format, FormatProvider, DateTimeStyles);
		return parsedValue;
	}
}
#endif