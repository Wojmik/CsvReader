using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration
{
	/// <summary>
	/// Property serializer with fixed deserializer
	/// </summary>
	/// <typeparam name="TRecord">Type of records read from csv</typeparam>
	/// <typeparam name="TProperty">Type of property</typeparam>
	/// <typeparam name="TSerializerConfigurator">Type of deserializer configurator</typeparam>
	public sealed class PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> : PropertyConfigurationBase<TRecord, TProperty>
		where TSerializerConfigurator : DeserializerConfigurationBase<TProperty>
	{
		private TSerializerConfigurator SerializerConfigurator { get; }

		internal PropertyConfigurationFixedSerializer(WojciechMikołajewicz.CsvReader.RecordConfiguration recordConfiguration, Expression<Func<TRecord, TProperty>> selector, Func<PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator>, TSerializerConfigurator> createSerializerConfigurator)
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

		/// <summary>
		/// Exposes ability of configuring deserializer
		/// </summary>
		/// <param name="configureSerializerMethod">Deserializer configuration delegate</param>
		/// <returns>This configuration object for methods chaining</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> ConfigureDeserializer(Action<TSerializerConfigurator> configureSerializerMethod)
		{
			configureSerializerMethod(SerializerConfigurator);

			return this;
		}

		/// <summary>
		/// Binds record's property to csv column by column name
		/// </summary>
		/// <param name="columnName">Csv column name</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="ArgumentNullException"><paramref name="columnName"/> is null</exception>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> BindToColumn(string columnName)
		{
			BindToColumnInternal(columnName);
			return this;
		}

		/// <summary>
		/// Binds record's property to zero based csv column number
		/// </summary>
		/// <param name="columnIndex">Zero based csv column number</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="columnIndex"/> is less than zero</exception>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TSerializerConfigurator> BindToColumn(int columnIndex)
		{
			BindToColumnInternal(columnIndex);
			return this;
		}

		/// <summary>
		/// Stets to ignore this record's property during csv deserialization. This property won't be set during deserialization.
		/// </summary>
		public void Ignore()
		{
			IgnoreInternal();
		}

		/// <summary>
		/// Tries build cell deserializer to property's type
		/// </summary>
		/// <param name="cellDeserializer">Cell deserializer</param>
		/// <returns>Did building succeed</returns>
		protected override bool TryBuildDeserializer(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
# endif
			out CellDeserializerBase<TProperty>? cellDeserializer)
		{
			return SerializerConfigurator.TryBuild(out cellDeserializer);
		}
	}
}