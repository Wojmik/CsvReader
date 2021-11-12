using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellTimeOnlyDeserializer : CellDeserializerFromMemoryBase<TimeOnly>
{
	private readonly IFormatProvider FormatProvider;

	private readonly DateTimeStyles DateTimeStyles;

	private readonly bool AllowEmpty;

	private readonly TimeOnly ValueForEmpty;

	public CellTimeOnlyDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, bool allowEmpty, TimeOnly valueForEmpty)
	{
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
			parsedValue = TimeOnly.Parse(value.Span, FormatProvider, DateTimeStyles);
		return parsedValue;
	}
}
#endif