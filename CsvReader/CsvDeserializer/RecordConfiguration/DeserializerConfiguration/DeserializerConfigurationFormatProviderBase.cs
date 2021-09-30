using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public abstract class DeserializerConfigurationFormatProviderBase<TRecord, TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationNotNullableBase<TRecord, TDeserialized, TDeserializerConfigurator>
		where TDeserialized : struct
		where TDeserializerConfigurator : DeserializerConfigurationFormatProviderBase<TRecord, TDeserialized, TDeserializerConfigurator>
	{
		private IFormatProvider? _FormatProvider;
		public IFormatProvider FormatProvider { get => _FormatProvider??RecordConfiguration.DefaultCulture; }

		public DeserializerConfigurationFormatProviderBase(PropertyConfigurationBase<TRecord, TDeserialized> propertyConfiguration)
			: base(propertyConfiguration)
		{ }

		public TDeserializerConfigurator SetFormatProvider(IFormatProvider formatProvider)
		{
			_FormatProvider = formatProvider;
			return (TDeserializerConfigurator)this;
		}
	}
}