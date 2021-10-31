using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellGuidDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<Guid>
#else
		CellDeserializerFromStringBase<Guid>
#endif
	{
		private readonly bool AllowEmpty;

		private readonly Guid ValueForEmpty;

		public CellGuidDeserializer(bool allowEmpty, Guid valueForEmpty)
		{
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
				parsedValue = Guid.Parse(value.Span);
			return parsedValue;
		}
#else
		protected override Guid DeserializeFromString(string value)
		{
			Guid parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = Guid.Parse(value);
			return parsedValue;
		}
#endif
	}
}