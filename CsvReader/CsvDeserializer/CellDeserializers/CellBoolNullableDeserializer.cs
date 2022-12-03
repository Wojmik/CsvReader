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
	sealed class CellBoolNullableDeserializer : CellDeserializerFromMemoryBase<bool?>
	{
		private readonly string _trueString;

		private readonly string _falseString;

		public CellBoolNullableDeserializer(string trueString, string falseString)
		{
			_trueString = trueString;
			_falseString = falseString;
		}

		protected override bool? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			bool? parsedValue;

			if(MemoryExtensions.Equals(value.Span, _trueString.AsSpan(), StringComparison.OrdinalIgnoreCase))
				parsedValue = true;
			else if(MemoryExtensions.Equals(value.Span, _falseString.AsSpan(), StringComparison.OrdinalIgnoreCase))
				parsedValue = false;
			else if(value.IsEmpty)
				parsedValue = default;
			else
				throw new FormatException("Value is not true string nor false string");
			return parsedValue;
		}
	}
}