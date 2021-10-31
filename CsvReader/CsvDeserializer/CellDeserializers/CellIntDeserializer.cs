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
	sealed class CellIntDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<int>
#else
		CellDeserializerFromStringBase<int>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly int ValueForEmpty;

		public CellIntDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, int valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override int DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			int parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = int.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override int DeserializeFromString(string value)
		{
			int parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = int.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}