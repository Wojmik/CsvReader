using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Class for custom cell to <typeparamref name="TDeserialized"/> deserializer configurators
	/// </summary>
	/// <typeparam name="TDeserialized">>Type deserializer deserializes to</typeparam>
	public class DeserializerConfigurationCustom<TDeserialized> : DeserializerConfigurationBase<TDeserialized>
	{
		/// <summary>
		/// Custom cell to <typeparamref name="TDeserialized"/> deserializer
		/// </summary>
		public CellDeserializerBase<TDeserialized> CellDeserializer { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		/// <param name="cellDeserializer">Custom cell to <typeparamref name="TDeserialized"/> deserializer</param>
		public DeserializerConfigurationCustom(BindingConfigurationBase bindingConfiguration, CellDeserializerBase<TDeserialized> cellDeserializer)
			: base(bindingConfiguration)
		{
			this.CellDeserializer = cellDeserializer;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
# endif
			out CellDeserializerBase<TDeserialized>? cellDeserializer)
		{
			cellDeserializer = CellDeserializer;
			return true;
		}
	}
}