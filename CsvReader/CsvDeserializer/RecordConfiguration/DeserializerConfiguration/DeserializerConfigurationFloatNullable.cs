using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationFloatNullable<TRecord> : DeserializerConfigurationNumberStyleFormatProviderNullableBase<TRecord, float, DeserializerConfigurationFloatNullable<TRecord>>
	{
		public DeserializerConfigurationFloatNullable(PropertyConfigurationBase<TRecord, float?> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.AllowThousands|NumberStyles.Float)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<float?>? cellDeserializer)
		{
			cellDeserializer = new CellFloatNullableDeserializer(NumberStyles, FormatProvider);
			return true;
		}
	}
}