using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellGuidFormattedNullableDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<Guid?>
#else
		CellDeserializerFromStringBase<Guid?>
#endif
	{
		private readonly string _format;

		public CellGuidFormattedNullableDeserializer(string format)
		{
			_format = format;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override Guid? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			Guid? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = Guid.ParseExact(value.Span, _format);
			return parsedValue;
		}
#else
		protected override Guid? DeserializeFromString(string value)
		{
			Guid? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = Guid.ParseExact(value, _format);
			return parsedValue;
		}
#endif
	}
}