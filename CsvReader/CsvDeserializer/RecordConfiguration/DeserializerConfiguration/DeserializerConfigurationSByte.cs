using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationSByte<TRecord> : DeserializerConfigurationNumberStyleFormatProviderBase<TRecord, sbyte, DeserializerConfigurationSByte<TRecord>>
	{
		public DeserializerConfigurationSByte(PropertyConfigurationBase<TRecord, sbyte> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Integer)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<sbyte>? cellDeserializer)
		{
			cellDeserializer = new CellSByteDeserializer(NumberStyles, FormatProvider, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}