using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationDateTime<TRecord> : DeserializerConfigurationDateTimeStyleFormatProviderBase<TRecord, DateTime, DeserializerConfigurationDateTime<TRecord>>
	{
		public DeserializerConfigurationDateTime(PropertyConfigurationBase<TRecord, DateTime> propertyConfiguration)
			: base(propertyConfiguration, DateTimeStyles.None)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<DateTime>? cellDeserializer)
		{
			cellDeserializer = new CellDateTimeDeserializer(FormatProvider, DateTimeStyles, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}