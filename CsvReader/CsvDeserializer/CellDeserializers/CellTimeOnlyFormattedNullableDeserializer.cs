using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellTimeOnlyFormattedNullableDeserializer : CellDeserializerFromMemoryBase<TimeOnly?>
{
	private readonly string Format;

	private readonly IFormatProvider FormatProvider;

	private readonly DateTimeStyles DateTimeStyles;

	public CellTimeOnlyFormattedNullableDeserializer(string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
	{
		Format = format;
		FormatProvider = formatProvider;
		DateTimeStyles = dateTimeStyles;
	}

	protected override TimeOnly? DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		TimeOnly? parsedValue = default;

		if (!value.IsEmpty)
			parsedValue = TimeOnly.ParseExact(value.Span, Format, FormatProvider, DateTimeStyles);
		return parsedValue;
	}
}
#endif