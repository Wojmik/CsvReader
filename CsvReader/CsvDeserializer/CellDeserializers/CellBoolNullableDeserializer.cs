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
		private readonly string TrueString;

		private readonly string FalseString;

		public CellBoolNullableDeserializer(string trueString, string falseString)
		{
			TrueString = trueString;
			FalseString = falseString;
		}

		protected override bool? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			bool? parsedValue;

			if(MemoryExtensions.Equals(value.Span, TrueString.AsSpan(), StringComparison.OrdinalIgnoreCase))
				parsedValue = true;
			else if(MemoryExtensions.Equals(value.Span, FalseString.AsSpan(), StringComparison.OrdinalIgnoreCase))
				parsedValue = false;
			else if(value.IsEmpty)
				parsedValue = default;
			else
				throw new FormatException("Value is not true string nor false string");
			return parsedValue;
		}
	}
}