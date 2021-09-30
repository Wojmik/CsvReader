using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationUIntNullable<TRecord> : DeserializerConfigurationNumberStyleFormatProviderNullableBase<TRecord, uint, DeserializerConfigurationUIntNullable<TRecord>>
	{
		public DeserializerConfigurationUIntNullable(PropertyConfigurationBase<TRecord, uint?> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Integer)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<uint?>? cellDeserializer)
		{
			cellDeserializer = new CellUIntNullableDeserializer(NumberStyles, FormatProvider);
			return true;
		}
	}
}