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
using System.Reflection;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.Helpers;

namespace WojciechMikołajewicz.CsvReader
{
	public class RecordConfiguration<TRecord>
	{
		public CultureInfo DefaultCulture { get; private set; }

		public bool HasHeaderRow { get; }

		public bool EmptyStringAsNull { get; private set; }

		public bool DeduplicateStrings { get; private set; }

		public StringDeduplicator StringDeduplicator { get; }

		private ChangeParameterNameExpressionVisitor ChangeParameterNameExpressionVisitor { get; }

		private Dictionary<string, BindingConfigurationBase<TRecord>> PropertyConfigurations { get; }

		private bool Building;

		internal RecordConfiguration(CsvDeserializerOptions options, StringDeduplicator stringDeduplicator)
		{
			DefaultCulture = options.DeserializationCulture;
			HasHeaderRow = options.HasHeaderRow;
			EmptyStringAsNull = options.EmptyStringAsNull;
			DeduplicateStrings = options.DeduplicateStrings;
			StringDeduplicator = stringDeduplicator;
			ChangeParameterNameExpressionVisitor = new ChangeParameterNameExpressionVisitor("0prm0");
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

		/// <summary>
		/// Gets string represenation of <paramref name="selector"/> expression with normalized name of the expression parameter.
		/// </summary>
		/// <typeparam name="TProperty">Type of selected property</typeparam>
		/// <param name="selector">Selecting expression</param>
		/// <returns>String representation of <paramref name="selector"/></returns>
		private string GetSelectorString<TProperty>(Expression<Func<TRecord, TProperty>> selector)
		{
			var oldParameter = selector.Parameters[0];
			var newBody = ChangeParameterNameExpressionVisitor.ChangeParameterName(selector.Body, oldParameter, out var newParameter);
			var newLambda = Expression.Lambda(newBody, newParameter);
			var stringRepresentation = newLambda.ToString();

			return stringRepresentation;
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
			var selectorString = GetSelectorString(selector);

			if(PropertyConfigurations.TryGetValue(selectorString, out var foundPropertyConfigurator)
				&& foundPropertyConfigurator is PropertyConfigurationBase<TRecord, TProperty> foundPropertyConfiguratorWithProperType)
			{
				foundPropertyConfiguratorWithProperType.ChangePropertySelector(selector);//Always set new property selector because property selector expression equality is not strict
				return foundPropertyConfiguratorWithProperType;
			}

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
			var selectorString = GetSelectorString(selector);
			PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator> propertyConfiguration;

			if(PropertyConfigurations.TryGetValue(selectorString, out var foundPropertyConfigurator))//Check if property configurator for this property exists already
			{
				if(foundPropertyConfigurator is PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator> foundPropertyConfiguratorWithProperType)//Check if founded property configurator has proper deserializer type
				{
					foundPropertyConfiguratorWithProperType.ChangePropertySelector(selector);//Always set new property selector because property selector expression equality is not strict
					propertyConfiguration = foundPropertyConfiguratorWithProperType;
				}
				else//Diferent serializer type so exchange property configurator with desired deserializer type
				{
					//Create new property configurator based on existing one
					propertyConfiguration = new PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator>(foundPropertyConfigurator, selector, createCustomDeserializerConfiguratorMethod);
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
		#region Automatic configuration
		internal void DiscoverRecordBinding()
		{
			var properties = typeof(TRecord).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

			foreach(var property in properties)
				AddPropertyInfoBinding(property);
		}

		internal void AddPropertyInfoBinding(PropertyInfo propertyInfo)
		{
			var parameterExpression = Expression.Parameter(typeof(TRecord), ChangeParameterNameExpressionVisitor.NewParameterName);
			var propertyExpression = Expression.Property(parameterExpression, propertyInfo);
			var selectorLambdaExpression = Expression.Lambda(propertyExpression, parameterExpression);
			BindingConfigurationBase<TRecord>? bindingConfigurationBase = null;

			var propertyType = propertyInfo.PropertyType;

			if(propertyType==typeof(int))
				bindingConfigurationBase = Property((Expression<Func<TRecord, int>>)selectorLambdaExpression);
			else if(propertyType==typeof(string))
				bindingConfigurationBase = Property((Expression<Func<TRecord, string?>>)selectorLambdaExpression);

			if(HasHeaderRow && bindingConfigurationBase!=null)
				bindingConfigurationBase.BindToColumnInternal(propertyInfo.Name);
		}
		#endregion
	}
}