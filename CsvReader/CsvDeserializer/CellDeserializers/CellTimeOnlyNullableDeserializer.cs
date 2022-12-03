using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

sealed class CellTimeOnlyNullableDeserializer : CellDeserializerFromMemoryBase<TimeOnly?>
{
	private readonly IFormatProvider _formatProvider;

	private readonly DateTimeStyles _dateTimeStyles;

	public CellTimeOnlyNullableDeserializer(IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
	{
		_formatProvider = formatProvider;
		_dateTimeStyles = dateTimeStyles;
	}

	protected override TimeOnly? DeserializeFromMemory(in ReadOnlyMemory<char> value)
	{
		TimeOnly? parsedValue = default;

		if (!value.IsEmpty)
			parsedValue = TimeOnly.Parse(value.Span, _formatProvider, _dateTimeStyles);
		return parsedValue;
	}
}
#endif