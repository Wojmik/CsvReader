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
		private readonly bool _allowEmpty;

		private readonly Guid _valueForEmpty;

		public CellGuidDeserializer(bool allowEmpty, Guid valueForEmpty)
		{
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override Guid DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			Guid parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = Guid.Parse(value.Span);
			return parsedValue;
		}
#else
		protected override Guid DeserializeFromString(string value)
		{
			Guid parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = Guid.Parse(value);
			return parsedValue;
		}
#endif
	}
}