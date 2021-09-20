using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using WojciechMikołajewicz.CsvReader.Helpers;
using System.Linq;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader
{
	public class RecordConfiguration<TRecord>
	{
		public CultureInfo DefaultCulture { get; private set; }

		public bool HasHeaderRow { get; }

		public bool EmptyStringAsNull { get; private set; }

		public bool DeduplicateStrings { get; private set; }

		public StringDeduplicator StringDeduplicator { get; }

		private Dictionary<string, BindingConfigurationBase<TRecord>> PropertyConfigurations { get; }

		private bool Building;

		internal RecordConfiguration(CsvDeserializerOptions options, StringDeduplicator stringDeduplicator)
		{
			DefaultCulture = options.DeserializationCulture;
			HasHeaderRow = options.HasHeaderRow;
			EmptyStringAsNull = options.EmptyStringAsNull;
			DeduplicateStrings = options.DeduplicateStrings;
			StringDeduplicator = stringDeduplicator;
			PropertyConfigurations = new Dictionary<string, BindingConfigurationBase<TRecord>>(StringComparer.Ordinal);
		}

		public RecordConfiguration<TRecord> SetDefaultCulture(CultureInfo defaultCulture)
		{
			CheckBuildingState();
			DefaultCulture = defaultCulture??throw new ArgumentNullException(nameof(defaultCulture));
			return this;
		}

		public RecordConfiguration<TRecord> SetEmptyStringBehavior(bool emptyAsNull)
		{
			CheckBuildingState();
			EmptyStringAsNull = emptyAsNull;
			return this;
		}

		public RecordConfiguration<TRecord> SetStringsDeduplicationgBehavior(bool deduplicateStrings)
		{
			CheckBuildingState();
			DeduplicateStrings = deduplicateStrings;
			return this;
		}

		internal IEnumerable<ColumnBinding<TRecord>> Build()
		{
			Building = true;

			foreach(var property in PropertyConfigurations)
				if(property.Value.TryBuild(out var binding))
					yield return binding!;
		}

		private void CheckBuildingState()
		{
			if(Building)
				throw new InvalidOperationException("Cannot change configuration while building phase");
		}

		#region Property
		public PropertyConfigurationFixedSerializer<TRecord, string?, DeserializerConfigurationString<TRecord>> Property(Expression<Func<TRecord, string?>> selector)
		{
			return PropertyWithCustomDeserializer<string?, DeserializerConfigurationString<TRecord>>(selector, propConf => new DeserializerConfigurationString<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, int, DeserializerConfigurationInt<TRecord>> Property(Expression<Func<TRecord, int>> selector)
		{
			return PropertyWithCustomDeserializer<int, DeserializerConfigurationInt<TRecord>>(selector, propConf => new DeserializerConfigurationInt<TRecord>(propConf));
		}

		public PropertyConfigurationBase<TRecord, TProperty> Property<TProperty>(Expression<Func<TRecord, TProperty>> selector)
		{
			var selectorString = selector.ToString();

			if(PropertyConfigurations.TryGetValue(selectorString, out var foundPropertyConfigurator)
				&& foundPropertyConfigurator is PropertyConfigurationBase<TRecord, TProperty> foundPropertyConfiguratorWithProperType)
				return foundPropertyConfiguratorWithProperType;

			throw new InvalidOperationException($"Property binding \"{selectorString}\" doesn't exist. Use {nameof(PropertyWithCustomDeserializer)} method to create one");
		}
		#endregion
		#region PropertyWithCustomDeserializer
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator> PropertyWithCustomDeserializer<TProperty, TDeserializerConfigurator>(
			Expression<Func<TRecord, TProperty>> selector,
			Func<PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator>, TDeserializerConfigurator> createCustomDeserializerConfiguratorMethod
			)
			where TDeserializerConfigurator : DeserializerConfigurationBase<TRecord, TProperty>
		{
			var selectorString = selector.ToString();
			PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator> propertyConfiguration;

			if(PropertyConfigurations.TryGetValue(selectorString, out var foundPropertyConfigurator))//Check if property configurator for this property exists already
			{
				if(foundPropertyConfigurator is PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator> foundPropertyConfiguratorWithProperType)//Check if founded property configurator has proper deserializer type
					propertyConfiguration = foundPropertyConfiguratorWithProperType;
				else//Diferent serializer type so exchange property configurator with desired deserializer type
				{
					//Create new property configurator based on existing one
					propertyConfiguration = new PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator>((PropertyConfigurationBase<TRecord, TProperty>)foundPropertyConfigurator, createCustomDeserializerConfiguratorMethod);
					//Override property configuration
					PropertyConfigurations[selectorString] = propertyConfiguration;
				}
			}
			else
			{
				propertyConfiguration = new PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator>(this, selector, createCustomDeserializerConfiguratorMethod);
				PropertyConfigurations.Add(selectorString, propertyConfiguration);
			}

			return propertyConfiguration;
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TRecord, TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, CellDeserializerFromMemorySequenceBase<TProperty> deserializer)
		{
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TRecord, TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TRecord, TProperty>(propConf, deserializer));
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TRecord, TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, CellDeserializerFromMemoryBase<TProperty> deserializer)
		{
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TRecord, TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TRecord, TProperty>(propConf, deserializer));
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TRecord, TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, CellDeserializerFromStringBase<TProperty> deserializer)
		{
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TRecord, TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TRecord, TProperty>(propConf, deserializer));
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TRecord, TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, Func<MemorySequenceSpan, TProperty> deserializeMethod)
		{
			var deserializer = new CellDeserializerFromMemorySequence<TProperty>(deserializeMethod);
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TRecord, TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TRecord, TProperty>(propConf, deserializer));
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TRecord, TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, Func<ReadOnlyMemory<char>, TProperty> deserializeMethod)
		{
			var deserializer = new CellDeserializerFromMemory<TProperty>(deserializeMethod);
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TRecord, TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TRecord, TProperty>(propConf, deserializer));
		}

		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TRecord, TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, Func<string, TProperty> deserializeMethod)
		{
			var deserializer = new CellDeserializerFromString<TProperty>(deserializeMethod);
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TRecord, TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TRecord, TProperty>(propConf, deserializer));
		}
		#endregion
	}
}