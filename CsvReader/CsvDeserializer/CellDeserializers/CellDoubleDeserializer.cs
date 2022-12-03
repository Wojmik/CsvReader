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
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly double _valueForEmpty;

		public CellDoubleDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, double valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override double DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			double parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = double.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override double DeserializeFromString(string value)
		{
			double parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = double.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}