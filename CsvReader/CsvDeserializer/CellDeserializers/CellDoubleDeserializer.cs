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
	sealed class CellDoubleDeserializer :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<double>
#else
		CellDeserializerFromStringBase<double>
#endif
	{
		private readonly NumberStyles NumberStyles;

		private readonly IFormatProvider FormatProvider;

		private readonly bool AllowEmpty;

		private readonly double ValueForEmpty;

		public CellDoubleDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, double valueForEmpty)
		{
			NumberStyles = numberStyles;
			FormatProvider = formatProvider;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override double DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			double parsedValue;

			if(AllowEmpty && value.IsEmpty)
				parsedValue = ValueForEmpty;
			else
				parsedValue = double.Parse(value.Span, NumberStyles, FormatProvider);
			return parsedValue;
		}
#else
		protected override double DeserializeFromString(string value)
		{
			double parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
				parsedValue = double.Parse(value, NumberStyles, FormatProvider);
			return parsedValue;
		}
#endif
	}
}