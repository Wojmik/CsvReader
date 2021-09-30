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
	sealed class CellEnumNullableDeserializer<TEnum> : CellDeserializerFromStringBase<TEnum?>
		where TEnum : struct
	{
		private readonly bool IgnoreCase;

		public CellEnumNullableDeserializer(bool ignoreCase)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
			IgnoreCase = ignoreCase;
		}

		protected override TEnum? DeserializeFromString(string value)
		{
			TEnum? parsedValue = default;

			if(!string.IsNullOrEmpty(value))
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