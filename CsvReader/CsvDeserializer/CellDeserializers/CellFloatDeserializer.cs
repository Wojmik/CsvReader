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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		CellDeserializerFromMemoryBase<float>
#else
		CellDeserializerFromStringBase<float>
#endif
	{
		private readonly NumberStyles _numberStyles;

		private readonly IFormatProvider _formatProvider;

		private readonly bool _allowEmpty;

		private readonly float _valueForEmpty;

		public CellFloatDeserializer(NumberStyles numberStyles, IFormatProvider formatProvider, bool allowEmpty, float valueForEmpty)
		{
			_numberStyles = numberStyles;
			_formatProvider = formatProvider;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
		protected override float DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			float parsedValue;

			if(_allowEmpty && value.IsEmpty)
				parsedValue = _valueForEmpty;
			else
				parsedValue = float.Parse(value.Span, _numberStyles, _formatProvider);
			return parsedValue;
		}
#else
		protected override float DeserializeFromString(string value)
		{
			float parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
				parsedValue = float.Parse(value, _numberStyles, _formatProvider);
			return parsedValue;
		}
#endif
	}
}