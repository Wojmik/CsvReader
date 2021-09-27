using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public abstract class DeserializerConfigurationBase<TRecord, TDeserialized>
	{
		protected RecordConfiguration<TRecord> RecordConfiguration { get => PropertyConfiguration.RecordConfiguration; }
		
		protected PropertyConfigurationBase<TRecord, TDeserialized> PropertyConfiguration { get; }

		public DeserializerConfigurationBase(PropertyConfigurationBase<TRecord, TDeserialized> propertyConfiguration)
		{
			this.PropertyConfiguration = propertyConfiguration;
		}

		internal abstract bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<TDeserialized>? cellDeserializer);
	}
}