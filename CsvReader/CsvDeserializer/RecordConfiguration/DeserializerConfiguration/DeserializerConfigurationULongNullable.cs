using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationULongNullable<TRecord> : DeserializerConfigurationNumberStyleFormatProviderNullableBase<TRecord, ulong, DeserializerConfigurationULongNullable<TRecord>>
	{
		public DeserializerConfigurationULongNullable(PropertyConfigurationBase<TRecord, ulong?> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Integer)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<ulong?>? cellDeserializer)
		{
			cellDeserializer = new CellULongNullableDeserializer(NumberStyles, FormatProvider);
			return true;
		}
	}
}