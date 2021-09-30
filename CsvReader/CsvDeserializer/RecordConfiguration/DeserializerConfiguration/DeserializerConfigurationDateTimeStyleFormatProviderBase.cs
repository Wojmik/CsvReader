using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public abstract class DeserializerConfigurationDateTimeStyleFormatProviderBase<TRecord, TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationNotNullableBase<TRecord, TDeserialized, TDeserializerConfigurator>
		where TDeserialized : struct
		where TDeserializerConfigurator : DeserializerConfigurationDateTimeStyleFormatProviderBase<TRecord, TDeserialized, TDeserializerConfigurator>
	{
		public DateTimeStyles DateTimeStyles { get; private set; }

		private IFormatProvider? _FormatProvider;
		public IFormatProvider FormatProvider { get => _FormatProvider??RecordConfiguration.DefaultCulture; }

		public DeserializerConfigurationDateTimeStyleFormatProviderBase(PropertyConfigurationBase<TRecord, TDeserialized> propertyConfiguration, DateTimeStyles defaultDateTimeStyles)
			: base(propertyConfiguration)
		{
			DateTimeStyles = defaultDateTimeStyles;
		}

		public TDeserializerConfigurator SetDateTimeStyles(DateTimeStyles dateTimeStyles)
		{
			DateTimeStyles = dateTimeStyles;
			return (TDeserializerConfigurator)this;
		}

		public TDeserializerConfigurator SetFormatProvider(IFormatProvider formatProvider)
		{
			_FormatProvider = formatProvider;
			return (TDeserializerConfigurator)this;
		}
	}
}