using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationDateTimeOffset<TRecord> : DeserializerConfigurationDateTimeStyleFormatProviderBase<TRecord, DateTimeOffset, DeserializerConfigurationDateTimeOffset<TRecord>>
	{
		public DeserializerConfigurationDateTimeOffset(PropertyConfigurationBase<TRecord, DateTimeOffset> propertyConfiguration)
			: base(propertyConfiguration, DateTimeStyles.None)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<DateTimeOffset>? cellDeserializer)
		{
			cellDeserializer = new CellDateTimeOffsetDeserializer(FormatProvider, DateTimeStyles, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}