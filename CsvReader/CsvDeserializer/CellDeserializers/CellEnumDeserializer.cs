﻿using System;
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
		private readonly bool IgnoreCase;

		private readonly bool AllowEmpty;

		private readonly TEnum ValueForEmpty;

		public CellEnumDeserializer(bool ignoreCase, bool allowEmpty, TEnum valueForEmpty)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
			IgnoreCase = ignoreCase;
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
		}

		protected override TEnum DeserializeFromString(string value)
		{
			TEnum parsedValue;

			if(AllowEmpty && string.IsNullOrEmpty(value))
				parsedValue = ValueForEmpty;
			else
			{
#if NETSTANDARD2_1_OR_GREATER
				parsedValue = Enum.Parse<TEnum>(value, IgnoreCase);
#else
				parsedValue = (TEnum)Enum.Parse(typeof(TEnum), value, IgnoreCase);
#endif
			}
			return parsedValue;
		}
	}
}