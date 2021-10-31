﻿using System;
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
		private readonly string Format;

		public CellGuidFormattedNullableDeserializer(string format)
		{
			Format = format;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override Guid? DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			Guid? parsedValue = default;

			if(!value.IsEmpty)
				parsedValue = Guid.ParseExact(value.Span, Format);
			return parsedValue;
		}
#else
		protected override Guid? DeserializeFromString(string value)
		{
			Guid? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
				parsedValue = Guid.ParseExact(value, Format);
			return parsedValue;
		}
#endif
	}
}