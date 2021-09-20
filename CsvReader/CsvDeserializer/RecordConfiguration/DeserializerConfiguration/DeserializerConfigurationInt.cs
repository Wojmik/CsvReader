using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationInt<TRecord> : DeserializerConfigurationStyleProviderBase<TRecord, int, DeserializerConfigurationInt<TRecord>>
	{
		public DeserializerConfigurationInt(PropertyConfigurationBase<TRecord, int> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Integer)
		{ }

		internal override bool TryBuild(out CellDeserializerBase<int>? cellDeserializer)
		{
			cellDeserializer = new CellIntDeserializer(NumberStyles, FormatProvider);
			return true;
		}
	}
}