using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Base class for cell to <typeparamref name="TDeserialized"/> deserializer configurators
	/// </summary>
	/// <typeparam name="TDeserialized">Type deserializer deserializes to</typeparam>
	public abstract class DeserializerConfigurationBase<TDeserialized>
	{
		/// <summary>
		/// Record deserializing configuration object
		/// </summary>
		protected WojciechMikołajewicz.CsvReader.RecordConfiguration RecordConfiguration { get => BindingConfiguration.RecordConfiguration; }
		
		/// <summary>
		/// Binding to column configuration object
		/// </summary>
		protected BindingConfigurationBase BindingConfiguration { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		protected DeserializerConfigurationBase(BindingConfigurationBase bindingConfiguration)
		{
			BindingConfiguration = bindingConfiguration;
		}

		internal abstract bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<TDeserialized>? cellDeserializer);
	}
}