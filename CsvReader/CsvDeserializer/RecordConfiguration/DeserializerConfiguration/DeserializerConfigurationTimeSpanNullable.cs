using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationTimeSpanNullable<TRecord> : DeserializerConfigurationFormatProviderNullableBase<TRecord, TimeSpan, DeserializerConfigurationTimeSpanNullable<TRecord>>
	{
		public DeserializerConfigurationTimeSpanNullable(PropertyConfigurationBase<TRecord, TimeSpan?> propertyConfiguration)
			: base(propertyConfiguration)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<TimeSpan?>? cellDeserializer)
		{
			cellDeserializer = new CellTimeSpanNullableDeserializer(FormatProvider);
			return true;
		}
	}
}