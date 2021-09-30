using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public abstract class DeserializerConfigurationDateTimeStyleFormatProviderNullableBase<TRecord, TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationBase<TRecord, TDeserialized?>
		where TDeserialized : struct
		where TDeserializerConfigurator : DeserializerConfigurationDateTimeStyleFormatProviderNullableBase<TRecord, TDeserialized, TDeserializerConfigurator>
	{
		public DateTimeStyles DateTimeStyles { get; private set; }

		private IFormatProvider? _FormatProvider;
		public IFormatProvider FormatProvider { get => _FormatProvider??RecordConfiguration.DefaultCulture; }

		public DeserializerConfigurationDateTimeStyleFormatProviderNullableBase(PropertyConfigurationBase<TRecord, TDeserialized?> propertyConfiguration, DateTimeStyles defaultDateTimeStyles)
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