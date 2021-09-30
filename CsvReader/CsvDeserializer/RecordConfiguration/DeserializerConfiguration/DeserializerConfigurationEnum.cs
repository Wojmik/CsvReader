using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationEnum<TRecord, TEnum> : DeserializerConfigurationNotNullableBase<TRecord, TEnum, DeserializerConfigurationEnum<TRecord, TEnum>>
		where TEnum : struct
	{
		public bool IgnoreCase { get; private set; }

		public DeserializerConfigurationEnum(PropertyConfigurationBase<TRecord, TEnum> propertyConfiguration)
			: base(propertyConfiguration)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
			IgnoreCase = RecordConfiguration.EnumsIgnoreCase;
		}

		public DeserializerConfigurationEnum<TRecord, TEnum> SetIgnoreCase(bool ignoreCase)
		{
			IgnoreCase = ignoreCase;
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<TEnum>? cellDeserializer)
		{
			cellDeserializer = new CellEnumDeserializer<TEnum>(IgnoreCase, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}