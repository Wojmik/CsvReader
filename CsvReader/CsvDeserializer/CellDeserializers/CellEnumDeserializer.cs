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
	sealed class CellEnumDeserializer<TEnum> : CellDeserializerFromStringBase<TEnum>
		where TEnum : struct
	{
		private readonly bool _ignoreCase;

		private readonly bool _allowEmpty;

		private readonly TEnum _valueForEmpty;

		public CellEnumDeserializer(bool ignoreCase, bool allowEmpty, TEnum valueForEmpty)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
			_ignoreCase = ignoreCase;
			_allowEmpty = allowEmpty;
			_valueForEmpty = valueForEmpty;
		}

		protected override TEnum DeserializeFromString(string value)
		{
			TEnum parsedValue;

			if(_allowEmpty && string.IsNullOrEmpty(value))
				parsedValue = _valueForEmpty;
			else
			{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
				parsedValue = Enum.Parse<TEnum>(value, _ignoreCase);
#else
				parsedValue = (TEnum)Enum.Parse(typeof(TEnum), value, _ignoreCase);
#endif
			}
			return parsedValue;
		}
	}
}