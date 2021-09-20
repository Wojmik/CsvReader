using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration
{
	public sealed class PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> : PropertyConfigurationBase<TRecord, TProperty>
		where TSerializerConfigurator : DeserializerConfigurationBase<TRecord, TProperty>
	{
		private TSerializerConfigurator SerializerConfigurator { get; }

		internal PropertyConfigurationFixedSerializer(RecordConfiguration<TRecord> recordConfiguration, Expression<Func<TRecord, TProperty>> selector, Func<PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator>, TSerializerConfigurator> createSerializerConfigurator)
			: base(recordConfiguration, selector)
		{
			this.SerializerConfigurator = createSerializerConfigurator(this);
		}

		internal PropertyConfigurationFixedSerializer(PropertyConfigurationBase<TRecord, TProperty> propertyConfiguration, Func<PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator>, TSerializerConfigurator> createSerializerConfigurator)
			: this(propertyConfiguration.RecordConfiguration, propertyConfiguration.PropertySelector, createSerializerConfigurator)
		{
			this.ColumnName = propertyConfiguration.ColumnName;
			this.ColumnIndex = propertyConfiguration.ColumnIndex;
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> ConfigureDeserializer(Action<TSerializerConfigurator> configureSerializerMethod)
		{
			configureSerializerMethod(SerializerConfigurator);

			return this;
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> BindToColumn(string columnName)
		{
			ColumnName = columnName??throw new ArgumentNullException(nameof(columnName));
			ColumnIndex = -1;

			return this;
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> BindToColumn(int columnIndex)
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
			return SerializerConfigurator.TryBuild(out cellDeserializer);
		}
	}
}