using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellBoolDeserializer : CellDeserializerFromMemoryBase<bool>
	{
		private readonly string _trueString;

		private readonly string _falseString;

		private readonly bool _allowEmpty;

		private readonly bool _valueForEmpty;

		public CellBoolDeserializer(string trueString, string falseString, bool allowEmpty, bool valueForEmpty)
		{
			_trueString = trueString;
			_falseString = falseString;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

		protected override bool DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			bool parsedValue;

			if(MemoryExtensions.Equals(value.Span, _trueString.AsSpan(), StringComparison.OrdinalIgnoreCase))
				parsedValue = true;
			else if(MemoryExtensions.Equals(value.Span, _falseString.AsSpan(), StringComparison.OrdinalIgnoreCase))
				parsedValue = false;
			else if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else 
				throw new FormatException("Value is not true string nor false string");
			return parsedValue;
		}
	}
}