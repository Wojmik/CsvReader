using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Base class for cell to not nullable <typeparamref name="TDeserialized"/> deserializers
	/// </summary>
	/// <typeparam name="TDeserialized">Type deserializer deserializes to</typeparam>
	/// <typeparam name="TDeserializerConfigurator">Type of deserializer configurator</typeparam>
	public abstract class DeserializerConfigurationNotNullableBase<TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationBase<TDeserialized>
		where TDeserialized : struct
		where TDeserializerConfigurator : DeserializerConfigurationNotNullableBase<TDeserialized, TDeserializerConfigurator>
	{
		/// <summary>
		/// Is empty cell allowed
		/// </summary>
		public bool AllowEmpty { get; private set; }

		/// <summary>
		/// <typeparamref name="TDeserialized"/> value for empty cell
		/// </summary>
		public TDeserialized ValueForEmpty { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		protected DeserializerConfigurationNotNullableBase(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{ }

		/// <summary>
		/// Sets allowness of empty cell
		/// </summary>
		/// <param name="allowEmpty">Allow empty cells or not</param>
		/// <param name="valueForEmpty">Empty cell equivalent value</param>
		/// <returns>This configuration object for methods chaining</returns>
		public TDeserializerConfigurator AllowEmptyValue(bool allowEmpty = true, TDeserialized valueForEmpty = default)
		{
			AllowEmpty = allowEmpty;
			ValueForEmpty = valueForEmpty;
			return (TDeserializerConfigurator)this;
		}
	}
}