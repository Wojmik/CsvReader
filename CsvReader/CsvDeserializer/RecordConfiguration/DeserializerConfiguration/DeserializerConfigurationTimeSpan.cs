using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationTimeSpan<TRecord> : DeserializerConfigurationFormatProviderBase<TRecord, TimeSpan, DeserializerConfigurationTimeSpan<TRecord>>
	{
		public DeserializerConfigurationTimeSpan(PropertyConfigurationBase<TRecord, TimeSpan> propertyConfiguration)
			: base(propertyConfiguration)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<TimeSpan>? cellDeserializer)
		{
			cellDeserializer = new CellTimeSpanDeserializer(FormatProvider, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}