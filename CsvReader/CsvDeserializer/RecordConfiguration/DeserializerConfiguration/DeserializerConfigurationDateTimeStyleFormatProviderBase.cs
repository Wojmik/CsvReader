using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Base deserializer configurator for date types
	/// </summary>
	/// <typeparam name="TDeserialized">Type deserializer deserializes to</typeparam>
	/// <typeparam name="TDeserializerConfigurator">Type of deserializer configurator</typeparam>
	public abstract class DeserializerConfigurationDateTimeStyleFormatProviderBase<TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationNotNullableBase<TDeserialized, TDeserializerConfigurator>
		where TDeserialized : struct
		where TDeserializerConfigurator : DeserializerConfigurationDateTimeStyleFormatProviderBase<TDeserialized, TDeserializerConfigurator>
	{
		private DateTimeStyles? _DateTimeStyles;
		/// <summary>
		/// Date styles used during parsing cell value to a date
		/// </summary>
		public DateTimeStyles DateTimeStyles { get => _DateTimeStyles??RecordConfiguration.DefaultDateStyles; }

		private IFormatProvider? _FormatProvider;
		/// <summary>
		/// Format provider used during parsing cell value to target type
		/// </summary>
		public IFormatProvider FormatProvider { get => _FormatProvider??RecordConfiguration.DefaultCulture; }

		/// <summary>
		/// Format used during parsing cell value to target type. If null, standard formats are used.
		/// </summary>
		public string? Format { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		protected DeserializerConfigurationDateTimeStyleFormatProviderBase(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{ }

		/// <summary>
		/// Sets date styles used during parsing cell value to a date
		/// </summary>
		/// <param name="dateTimeStyles">Desired date styles for parsing cell value to a date</param>
		/// <returns>This configuration object for methods chaining</returns>
		public TDeserializerConfigurator SetDateTimeStyles(DateTimeStyles dateTimeStyles)
		{
			_DateTimeStyles = dateTimeStyles;
			return (TDeserializerConfigurator)this;
		}

		/// <summary>
		/// Sets format provider used during parsing cell value to <typeparamref name="TDeserialized"/> type. If null, <see cref="WojciechMikołajewicz.CsvReader.RecordConfiguration.DefaultCulture"/> is used.
		/// </summary>
		/// <param name="formatProvider">Desired format provider for parsing cell value to <typeparamref name="TDeserialized"/> type</param>
		/// <returns>This configuration object for methods chaining</returns>
		public TDeserializerConfigurator SetFormatProvider(IFormatProvider? formatProvider)
		{
			_FormatProvider = formatProvider;
			return (TDeserializerConfigurator)this;
		}

		/// <summary>
		/// Sets format used during parsing cell value to <typeparamref name="TDeserialized"/> type. If null, standard formats are used.
		/// </summary>
		/// <param name="format">Desired format for parsing cell value to <typeparamref name="TDeserialized"/> type</param>
		/// <returns>This configuration object for methods chaining</returns>
		public TDeserializerConfigurator SetFormat(string? format)
		{
			Format = format;
			return (TDeserializerConfigurator)this;
		}
	}
}