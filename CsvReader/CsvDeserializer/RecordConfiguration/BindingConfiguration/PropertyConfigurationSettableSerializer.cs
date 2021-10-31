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
	/// Class for configuring property bindig to a csv column
	/// </summary>
	/// <typeparam name="TRecord">Type of records read from csv</typeparam>
	/// <typeparam name="TProperty">Type of record's property</typeparam>
	public class PropertyConfigurationSettableSerializer<TRecord, TProperty> : PropertyConfigurationBase<TRecord, TProperty>
	{
		private DeserializerConfigurationBase<TProperty>? SerializerConfigurator { get; set; }

		internal PropertyConfigurationSettableSerializer(WojciechMikołajewicz.CsvReader.RecordConfiguration recordConfiguration, Expression<Func<TRecord, TProperty>> selector)
			: base(recordConfiguration, selector)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <typeparam name="TSerializerConfigurator">Type of deserializer configurator</typeparam>
		/// <param name="createSerializer">Method providing instance of deserializer configurator</param>
		/// <param name="configureSerializerMethod">Method for configuring deserializer configurator</param>
		/// <returns></returns>
		public PropertyConfigurationSettableSerializer<TRecord, TProperty> ConfigureDeserializer<TSerializerConfigurator>(Func<TSerializerConfigurator> createSerializer, Action<TSerializerConfigurator> configureSerializerMethod)
			where TSerializerConfigurator : DeserializerConfigurationBase<TProperty>
		{
			var serializerConfigurator = createSerializer();
			SerializerConfigurator = serializerConfigurator;
			configureSerializerMethod(serializerConfigurator);

			return this;
		}

		/// <summary>
		/// Sets property bindings to a column by name
		/// </summary>
		/// <param name="columnName">Name of the column property will be bind to</param>
		/// <returns>This property binding configurator for methods chaining</returns>
		public PropertyConfigurationSettableSerializer<TRecord, TProperty> BindToColumn(string columnName)
		{
			ColumnName = columnName??throw new ArgumentNullException(nameof(columnName));
			ColumnIndex = -1;

			return this;
		}

		/// <summary>
		/// Sets property bindings to a column by zero based column index
		/// </summary>
		/// <param name="columnIndex">Zero based column index property will be bind to</param>
		/// <returns>This property binding configurator for methods chaining</returns>
		public PropertyConfigurationSettableSerializer<TRecord, TProperty> BindToColumn(int columnIndex)
		{
			if(columnIndex<0)
				throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, $"{nameof(columnIndex)} cannot be less than zero");
			ColumnName = null;
			ColumnIndex = columnIndex;

			return this;
		}

		/// <summary>
		/// Clears property binding which means property will be ignored during deserialization because lack of binding to a csv column
		/// </summary>
		public void Ignore()
		{
			IgnoreInternal();
		}

		/// <summary>
		/// Tries build cell deserializer to property's type
		/// </summary>
		/// <param name="cellDeserializer">Cell deserializer</param>
		/// <returns></returns>
		protected override bool TryBuildDeserializer(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
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