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
	sealed class CellFloatDeserializer :
#if NETSTANDARD2_1_OR_GREATER
		CellDeserializerFromMemoryBase<float>
#else
		CellDeserializerFromStringBase<float>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly float ValueForEmpty;

		public CellFloatDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, float valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER
		protected override float DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			float parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = float.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override float DeserializeFromString(string value)
		{
			float parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = float.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}