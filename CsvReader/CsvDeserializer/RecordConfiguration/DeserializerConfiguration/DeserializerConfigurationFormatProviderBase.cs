using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Base deserializer configurator for <see cref="IFormatProvider"/> aware types
	/// </summary>
	/// <typeparam name="TDeserialized">Type deserializer deserializes to</typeparam>
	/// <typeparam name="TDeserializerConfigurator">Type of deserializer configurator</typeparam>
	public abstract class DeserializerConfigurationFormatProviderBase<TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationNotNullableBase<TDeserialized, TDeserializerConfigurator>
		where TDeserialized : struct
		where TDeserializerConfigurator : DeserializerConfigurationFormatProviderBase<TDeserialized, TDeserializerConfigurator>
	{
		private IFormatProvider? _FormatProvider;
		/// <summary>
		/// Format provider used during parsing cell value to target type
		/// </summary>
		public IFormatProvider FormatProvider { get => _FormatProvider??RecordConfiguration.DefaultCulture; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		protected DeserializerConfigurationFormatProviderBase(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{ }

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
	}
}