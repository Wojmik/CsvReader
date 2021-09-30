using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationUShortNullable<TRecord> : DeserializerConfigurationNumberStyleFormatProviderNullableBase<TRecord, ushort, DeserializerConfigurationUShortNullable<TRecord>>
	{
		public DeserializerConfigurationUShortNullable(PropertyConfigurationBase<TRecord, ushort?> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Integer)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<ushort?>? cellDeserializer)
		{
			cellDeserializer = new CellUShortNullableDeserializer(NumberStyles, FormatProvider);
			return true;
		}
	}
}