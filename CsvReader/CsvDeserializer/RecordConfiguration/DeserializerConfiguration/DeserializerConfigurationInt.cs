using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationInt<TRecord> : DeserializerConfigurationNumberStyleFormatProviderBase<TRecord, int, DeserializerConfigurationInt<TRecord>>
	{
		public DeserializerConfigurationInt(PropertyConfigurationBase<TRecord, int> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Integer)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<int>? cellDeserializer)
		{
			cellDeserializer = new CellIntDeserializer(NumberStyles, FormatProvider, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}