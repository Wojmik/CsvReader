using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationIntNullable<TRecord> : DeserializerConfigurationNumberStyleFormatProviderNullableBase<TRecord, int, DeserializerConfigurationIntNullable<TRecord>>
	{
		public DeserializerConfigurationIntNullable(PropertyConfigurationBase<TRecord, int?> propertyConfiguration)
			: base(propertyConfiguration, NumberStyles.Integer)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<int?>? cellDeserializer)
		{
			cellDeserializer = new CellIntNullableDeserializer(NumberStyles, FormatProvider);
			return true;
		}
	}
}