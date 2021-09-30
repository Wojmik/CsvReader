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

		internal PropertyConfigurationFixedSerializer(BindingConfigurationBase<TRecord> bindingConfiguration, Expression<Func<TRecord, TProperty>> selector, Func<PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator>, TSerializerConfigurator> createSerializerConfigurator)
			: this(bindingConfiguration.RecordConfiguration, selector, createSerializerConfigurator)
		{
			this.ColumnName = bindingConfiguration.ColumnName;
			this.ColumnIndex = bindingConfiguration.ColumnIndex;
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> ConfigureDeserializer(Action<TSerializerConfigurator> configureSerializerMethod)
		{
			configureSerializerMethod(SerializerConfigurator);

			return this;
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> BindToColumn(string columnName)
		{
			BindToColumnInternal(columnName);
			return this;
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> BindToColumn(int columnIndex)
		{
			BindToColumnInternal(columnIndex);
			return this;
		}

		public void Ignore()
		{
			IgnoreInternal();
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