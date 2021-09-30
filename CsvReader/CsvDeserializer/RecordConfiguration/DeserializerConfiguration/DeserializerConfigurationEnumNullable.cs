using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationEnumNullable<TRecord, TEnum> : DeserializerConfigurationBase<TRecord, TEnum?>
		where TEnum : struct
	{
		public bool IgnoreCase { get; private set; }

		public DeserializerConfigurationEnumNullable(PropertyConfigurationBase<TRecord, TEnum?> propertyConfiguration)
			: base(propertyConfiguration)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
			IgnoreCase = RecordConfiguration.EnumsIgnoreCase;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<TEnum?>? cellDeserializer)
		{
			cellDeserializer = new CellEnumNullableDeserializer<TEnum>(IgnoreCase);
			return true;
		}
	}
}