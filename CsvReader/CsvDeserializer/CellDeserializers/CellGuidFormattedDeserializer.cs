using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellGuidFormattedDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<Guid>
#else
		CellDeserializerFromStringBase<Guid>
#endif
	{
		private readonly string Format;

		private readonly bool AllowEmpty;

		private readonly Guid ValueForEmpty;

		public CellGuidFormattedDeserializer(string format, bool allowEmpty, Guid valueForEmpty)
		{
			Format = format;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override Guid DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			Guid parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = Guid.ParseExact(value.Span, Format);
			return parsedValue;
		}
#else
		protected override Guid DeserializeFromString(string value)
		{
			Guid parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = Guid.ParseExact(value, Format);
			return parsedValue;
		}
#endif
	}
}