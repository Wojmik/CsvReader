using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public abstract class DeserializerConfigurationNotNullableBase<TRecord, TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationBase<TRecord, TDeserialized>
		where TDeserialized : struct
		where TDeserializerConfigurator : DeserializerConfigurationNotNullableBase<TRecord, TDeserialized, TDeserializerConfigurator>
	{
		public bool AllowEmpty { get; private set; }

		public TDeserialized ValueForEmpty { get; private set; }

		public DeserializerConfigurationNotNullableBase(PropertyConfigurationBase<TRecord, TDeserialized> propertyConfiguration)
			: base(propertyConfiguration)
		{ }

		public TDeserializerConfigurator AllowEmptyValue(bool allowEmpty = true, TDeserialized valueForEmpty = default)
		{
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
			return (TDeserializerConfigurator)this;
		}
	}
}