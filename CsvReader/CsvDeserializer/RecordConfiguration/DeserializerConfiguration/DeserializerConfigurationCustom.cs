using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationCustom<TRecord, TDeserialized> : DeserializerConfigurationBase<TRecord, TDeserialized>
	{
		public CellDeserializerBase<TDeserialized> CellDeserializer { get; }

		public DeserializerConfigurationCustom(PropertyConfigurationBase<TRecord, TDeserialized> propertyConfiguration, CellDeserializerBase<TDeserialized> cellDeserializer)
			: base(propertyConfiguration)
		{
			this.CellDeserializer = cellDeserializer;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
# endif
			out CellDeserializerBase<TDeserialized>? cellDeserializer)
		{
			cellDeserializer = CellDeserializer;
			return true;
		}
	}
}