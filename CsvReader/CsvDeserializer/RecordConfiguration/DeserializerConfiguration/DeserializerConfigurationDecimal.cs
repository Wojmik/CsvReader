using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationDecimal<TRecord> : DeserializerConfigurationNumberStyleFormatProviderBase<TRecord, decimal, DeserializerConfigurationDecimal<TRecord>>
	{
		public DeserializerConfigurationDecimal(PropertyConfigurationBase<TRecord, decimal> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Number)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<decimal>? cellDeserializer)
		{
			cellDeserializer = new CellDecimalDeserializer(NumberStyles, FormatProvider, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}