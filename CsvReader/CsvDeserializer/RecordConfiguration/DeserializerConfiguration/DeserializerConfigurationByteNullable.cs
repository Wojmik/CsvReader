using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationByteNullable<TRecord> : DeserializerConfigurationNumberStyleFormatProviderNullableBase<TRecord, byte, DeserializerConfigurationByteNullable<TRecord>>
	{
		public DeserializerConfigurationByteNullable(PropertyConfigurationBase<TRecord, byte?> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Integer)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<byte?>? cellDeserializer)
		{
			cellDeserializer = new CellByteNullableDeserializer(NumberStyles, FormatProvider);
			return true;
		}
	}
}