using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public abstract class DeserializerConfigurationStyleProviderBase<TRecord, TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationBase<TRecord, TDeserialized>
		where TDeserializerConfigurator : DeserializerConfigurationStyleProviderBase<TRecord, TDeserialized, TDeserializerConfigurator>
	{
		public NumberStyles NumberStyles { get; private set; }

		private IFormatProvider? _FormatProvider;
		public IFormatProvider FormatProvider { get => _FormatProvider??RecordConfiguration.DefaultCulture; }

		public DeserializerConfigurationStyleProviderBase(PropertyConfigurationBase<TRecord, TDeserialized> propertyConfiguration, NumberStyles defaultNumberStyles)
			: base(propertyConfiguration)
		{
			this.NumberStyles = defaultNumberStyles;
		}

		public TDeserializerConfigurator SetNumberStyles(NumberStyles numberStyles)
		{
			NumberStyles = numberStyles;
			return (TDeserializerConfigurator)this;
		}

		public TDeserializerConfigurator SetFormatProvider(IFormatProvider formatProvider)
		{
			_FormatProvider = formatProvider;
			return (TDeserializerConfigurator)this;
		}
	}
}