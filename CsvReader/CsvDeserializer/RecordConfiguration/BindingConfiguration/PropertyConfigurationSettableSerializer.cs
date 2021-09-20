using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration
{
	public class PropertyConfigurationSettableSerializer<TRecord, TProperty> : PropertyConfigurationBase<TRecord, TProperty>
	{
		private DeserializerConfigurationBase<TRecord, TProperty>? SerializerConfigurator { get; set; }

		internal PropertyConfigurationSettableSerializer(RecordConfiguration<TRecord> recordConfiguration, Expression<Func<TRecord, TProperty>> selector)
			: base(recordConfiguration, selector)
		{ }

		public PropertyConfigurationSettableSerializer<TRecord, TProperty> ConfigureDeserializer<TSerializerConfigurator>(Func<TSerializerConfigurator> createSerializer, Action<TSerializerConfigurator> configureSerializerMethod)
			where TSerializerConfigurator : DeserializerConfigurationBase<TRecord, TProperty>
		{
			var serializerConfigurator = createSerializer();
			SerializerConfigurator = serializerConfigurator;
			configureSerializerMethod(serializerConfigurator);

			return this;
		}

		public PropertyConfigurationSettableSerializer<TRecord, TProperty> BindToColumn(string columnName)
		{
			ColumnName = columnName??throw new ArgumentNullException(nameof(columnName));
			ColumnIndex = -1;

			return this;
		}

		public PropertyConfigurationSettableSerializer<TRecord, TProperty> BindToColumn(int columnIndex)
		{
			if(columnIndex<0)
				throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, $"{nameof(columnIndex)} cannot be less than zero");
			ColumnName = null;
			ColumnIndex = columnIndex;

			return this;
		}

		public void Ignore()
		{
			ColumnName = null;
			ColumnIndex = -1;
		}

		protected override bool TryBuildDeserializer(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
# endif
			out CellDeserializerBase<TProperty>? cellDeserializer)
		{
			if(SerializerConfigurator==null)
			{
				cellDeserializer = null;
				return false;
			}

			return SerializerConfigurator.TryBuild(out cellDeserializer);
		}
	}
}