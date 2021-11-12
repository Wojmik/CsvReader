using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellTimeOnlyNullableDeserializer : CellDeserializerFromMemoryBase<TimeOnly?>
{
	private readonly IFormatProvider FormatProvider;

	private readonly DateTimeStyles DateTimeStyles;

	public CellTimeOnlyNullableDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
	{
		FormatProvider = formatProvider;
		DateTimeStyles = dateTimeStyles;
	}

	protected override TimeOnly? DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		TimeOnly? parsedValue = default;

		if (!value.IsEmpty)
			parsedValue = TimeOnly.Parse(value.Span, FormatProvider, DateTimeStyles);
		return parsedValue;
	}
}
#endif