using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationLongNullable<TRecord> : DeserializerConfigurationNumberStyleFormatProviderNullableBase<TRecord, long, DeserializerConfigurationLongNullable<TRecord>>
	{
		public DeserializerConfigurationLongNullable(PropertyConfigurationBase<TRecord, long?> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Integer)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<long?>? cellDeserializer)
		{
			cellDeserializer = new CellLongNullableDeserializer(NumberStyles, FormatProvider);
			return true;
		}
	}
}