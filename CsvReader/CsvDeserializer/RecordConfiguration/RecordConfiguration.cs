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

		public bool EmptyAsNull { get; private set; }

		public bool DeduplicateStrings { get; private set; }

		public ByteArrayEncoding ByteArrayEncoding { get; private set; }

		public bool EnumsIgnoreCase { get; private set; }

		public StringDeduplicator StringDeduplicator { get; }

		private ChangeParameterNameExpressionVisitor ChangeParameterNameExpressionVisitor { get; }

		private Dictionary<string, BindingConfigurationBase<TRecord>> PropertyConfigurations { get; }

		private bool Building;

		internal RecordConfiguration(CsvDeserializerOptions options, StringDeduplicator stringDeduplicator)
		{
			DefaultCulture = options.DeserializationCulture;
			HasHeaderRow = options.HasHeaderRow;
			EmptyAsNull = options.EmptyAsNull;
			DeduplicateStrings = options.DeduplicateStrings;
			StringDeduplicator = stringDeduplicator;
			ByteArrayEncoding = ByteArrayEncoding.Base64;
			EnumsIgnoreCase = true;
			ChangeParameterNameExpressionVisitor = new ChangeParameterNameExpressionVisitor("0prm0");
			PropertyConfigurations = new Dictionary<string, BindingConfigurationBase<TRecord>>(StringComparer.Ordinal);
		}

		public RecordConfiguration<TRecord> SetDefaultCulture(CultureInfo defaultCulture)
		{
			CheckBuildingState();
			DefaultCulture = defaultCulture??throw new ArgumentNullException(nameof(defaultCulture));
			return this;
		}

		public RecordConfiguration<TRecord> SetEmptyBehavior(bool emptyAsNull)
		{
			CheckBuildingState();
			EmptyAsNull = emptyAsNull;
			return this;
		}

		public RecordConfiguration<TRecord> SetStringsDeduplicationgBehavior(bool deduplicateStrings)
		{
			CheckBuildingState();
			DeduplicateStrings = deduplicateStrings;
			return this;
		}

		public RecordConfiguration<TRecord> SetByteArrayEncoding(ByteArrayEncoding byteArrayEncoding)
		{
			CheckBuildingState();
			ByteArrayEncoding = byteArrayEncoding;
			return this;
		}

		public RecordConfiguration<TRecord> SetEnumsIgnoreCaseBehavior(bool enumsIgnoreCase)
		{
			CheckBuildingState();
			EnumsIgnoreCase = enumsIgnoreCase;
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

		public RecordConfiguration<TRecord> ClearAlBindings()
		{
			foreach(var property in PropertyConfigurations)
				property.Value.IgnoreInternal();
			return this;
		}

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
			Type underlyingType;

			if(propertyType==typeof(string))
				bindingConfigurationBase = Property((Expression<Func<TRecord, string?>>)selectorLambdaExpression);
			else if(propertyType==typeof(bool))
				bindingConfigurationBase = Property((Expression<Func<TRecord, bool>>)selectorLambdaExpression);
			else if(propertyType==typeof(byte))
				bindingConfigurationBase = Property((Expression<Func<TRecord, byte>>)selectorLambdaExpression);
			else if(propertyType==typeof(sbyte))
				bindingConfigurationBase = Property((Expression<Func<TRecord, sbyte>>)selectorLambdaExpression);
			else if(propertyType==typeof(short))
				bindingConfigurationBase = Property((Expression<Func<TRecord, short>>)selectorLambdaExpression);
			else if(propertyType==typeof(ushort))
				bindingConfigurationBase = Property((Expression<Func<TRecord, ushort>>)selectorLambdaExpression);
			else if(propertyType==typeof(int))
				bindingConfigurationBase = Property((Expression<Func<TRecord, int>>)selectorLambdaExpression);
			else if(propertyType==typeof(uint))
				bindingConfigurationBase = Property((Expression<Func<TRecord, uint>>)selectorLambdaExpression);
			else if(propertyType==typeof(long))
				bindingConfigurationBase = Property((Expression<Func<TRecord, long>>)selectorLambdaExpression);
			else if(propertyType==typeof(ulong))
				bindingConfigurationBase = Property((Expression<Func<TRecord, ulong>>)selectorLambdaExpression);
			else if(propertyType==typeof(float))
				bindingConfigurationBase = Property((Expression<Func<TRecord, float>>)selectorLambdaExpression);
			else if(propertyType==typeof(double))
				bindingConfigurationBase = Property((Expression<Func<TRecord, double>>)selectorLambdaExpression);
			else if(propertyType==typeof(decimal))
				bindingConfigurationBase = Property((Expression<Func<TRecord, decimal>>)selectorLambdaExpression);
			else if(propertyType==typeof(DateTime))
				bindingConfigurationBase = Property((Expression<Func<TRecord, DateTime>>)selectorLambdaExpression);
			else if(propertyType==typeof(TimeSpan))
				bindingConfigurationBase = Property((Expression<Func<TRecord, TimeSpan>>)selectorLambdaExpression);
			else if(propertyType==typeof(DateTimeOffset))
				bindingConfigurationBase = Property((Expression<Func<TRecord, DateTimeOffset>>)selectorLambdaExpression);
			else if(propertyType==typeof(byte[]))
				bindingConfigurationBase = Property((Expression<Func<TRecord, byte[]?>>)selectorLambdaExpression);
			else if(propertyType==typeof(bool?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, bool?>>)selectorLambdaExpression);
			else if(propertyType==typeof(byte?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, byte?>>)selectorLambdaExpression);
			else if(propertyType==typeof(sbyte?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, sbyte?>>)selectorLambdaExpression);
			else if(propertyType==typeof(short?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, short?>>)selectorLambdaExpression);
			else if(propertyType==typeof(ushort?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, ushort?>>)selectorLambdaExpression);
			else if(propertyType==typeof(int?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, int?>>)selectorLambdaExpression);
			else if(propertyType==typeof(uint?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, uint?>>)selectorLambdaExpression);
			else if(propertyType==typeof(long?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, long?>>)selectorLambdaExpression);
			else if(propertyType==typeof(ulong?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, ulong?>>)selectorLambdaExpression);
			else if(propertyType==typeof(float?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, float?>>)selectorLambdaExpression);
			else if(propertyType==typeof(double?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, double?>>)selectorLambdaExpression);
			else if(propertyType==typeof(decimal?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, decimal?>>)selectorLambdaExpression);
			else if(propertyType==typeof(DateTime?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, DateTime?>>)selectorLambdaExpression);
			else if(propertyType==typeof(TimeSpan?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, TimeSpan?>>)selectorLambdaExpression);
			else if(propertyType==typeof(DateTimeOffset?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, DateTimeOffset?>>)selectorLambdaExpression);
			else if(propertyType.IsEnum)
			{
				Func<Expression<Func<TRecord, int>>, PropertyConfigurationFixedSerializer<TRecord, int, DeserializerConfigurationEnum<TRecord, int>>> func = PropertyEnum;
				var genericMethodDefinition = func.Method.GetGenericMethodDefinition();
				var methodInfo = genericMethodDefinition.MakeGenericMethod(propertyType);
				var restult = methodInfo.Invoke(this, new object[] { selectorLambdaExpression, });
				bindingConfigurationBase = (BindingConfigurationBase<TRecord>)restult;
			}
			else if((underlyingType=Nullable.GetUnderlyingType(propertyType))!=null && underlyingType.IsEnum)
			{
				Func<Expression<Func<TRecord, int?>>, PropertyConfigurationFixedSerializer<TRecord, int?, DeserializerConfigurationEnumNullable<TRecord, int>>> func = PropertyEnum;
				var genericMethodDefinition = func.Method.GetGenericMethodDefinition();
				var methodInfo = genericMethodDefinition.MakeGenericMethod(underlyingType);
				var restult = methodInfo.Invoke(this, new object[] { selectorLambdaExpression, });
				bindingConfigurationBase = (BindingConfigurationBase<TRecord>)restult;
			}

			if(HasHeaderRow && bindingConfigurationBase!=null)
				bindingConfigurationBase.BindToColumnInternal(propertyInfo.Name);
		}
		#endregion
		#region Property
		public PropertyConfigurationFixedSerializer<TRecord, string?, DeserializerConfigurationString<TRecord>> Property(Expression<Func<TRecord, string?>> selector)
		{
			return PropertyWithCustomDeserializer<string?, DeserializerConfigurationString<TRecord>>(selector, propConf => new DeserializerConfigurationString<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, bool, DeserializerConfigurationBool<TRecord>> Property(Expression<Func<TRecord, bool>> selector)
		{
			return PropertyWithCustomDeserializer<bool, DeserializerConfigurationBool<TRecord>>(selector, propConf => new DeserializerConfigurationBool<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, byte, DeserializerConfigurationByte<TRecord>> Property(Expression<Func<TRecord, byte>> selector)
		{
			return PropertyWithCustomDeserializer<byte, DeserializerConfigurationByte<TRecord>>(selector, propConf => new DeserializerConfigurationByte<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, sbyte, DeserializerConfigurationSByte<TRecord>> Property(Expression<Func<TRecord, sbyte>> selector)
		{
			return PropertyWithCustomDeserializer<sbyte, DeserializerConfigurationSByte<TRecord>>(selector, propConf => new DeserializerConfigurationSByte<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, short, DeserializerConfigurationShort<TRecord>> Property(Expression<Func<TRecord, short>> selector)
		{
			return PropertyWithCustomDeserializer<short, DeserializerConfigurationShort<TRecord>>(selector, propConf => new DeserializerConfigurationShort<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, ushort, DeserializerConfigurationUShort<TRecord>> Property(Expression<Func<TRecord, ushort>> selector)
		{
			return PropertyWithCustomDeserializer<ushort, DeserializerConfigurationUShort<TRecord>>(selector, propConf => new DeserializerConfigurationUShort<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, int, DeserializerConfigurationInt<TRecord>> Property(Expression<Func<TRecord, int>> selector)
		{
			return PropertyWithCustomDeserializer<int, DeserializerConfigurationInt<TRecord>>(selector, propConf => new DeserializerConfigurationInt<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, uint, DeserializerConfigurationUInt<TRecord>> Property(Expression<Func<TRecord, uint>> selector)
		{
			return PropertyWithCustomDeserializer<uint, DeserializerConfigurationUInt<TRecord>>(selector, propConf => new DeserializerConfigurationUInt<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, long, DeserializerConfigurationLong<TRecord>> Property(Expression<Func<TRecord, long>> selector)
		{
			return PropertyWithCustomDeserializer<long, DeserializerConfigurationLong<TRecord>>(selector, propConf => new DeserializerConfigurationLong<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, ulong, DeserializerConfigurationULong<TRecord>> Property(Expression<Func<TRecord, ulong>> selector)
		{
			return PropertyWithCustomDeserializer<ulong, DeserializerConfigurationULong<TRecord>>(selector, propConf => new DeserializerConfigurationULong<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, float, DeserializerConfigurationFloat<TRecord>> Property(Expression<Func<TRecord, float>> selector)
		{
			return PropertyWithCustomDeserializer<float, DeserializerConfigurationFloat<TRecord>>(selector, propConf => new DeserializerConfigurationFloat<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, double, DeserializerConfigurationDouble<TRecord>> Property(Expression<Func<TRecord, double>> selector)
		{
			return PropertyWithCustomDeserializer<double, DeserializerConfigurationDouble<TRecord>>(selector, propConf => new DeserializerConfigurationDouble<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, decimal, DeserializerConfigurationDecimal<TRecord>> Property(Expression<Func<TRecord, decimal>> selector)
		{
			return PropertyWithCustomDeserializer<decimal, DeserializerConfigurationDecimal<TRecord>>(selector, propConf => new DeserializerConfigurationDecimal<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, DateTime, DeserializerConfigurationDateTime<TRecord>> Property(Expression<Func<TRecord, DateTime>> selector)
		{
			return PropertyWithCustomDeserializer<DateTime, DeserializerConfigurationDateTime<TRecord>>(selector, propConf => new DeserializerConfigurationDateTime<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, TimeSpan, DeserializerConfigurationTimeSpan<TRecord>> Property(Expression<Func<TRecord, TimeSpan>> selector)
		{
			return PropertyWithCustomDeserializer<TimeSpan, DeserializerConfigurationTimeSpan<TRecord>>(selector, propConf => new DeserializerConfigurationTimeSpan<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, DateTimeOffset, DeserializerConfigurationDateTimeOffset<TRecord>> Property(Expression<Func<TRecord, DateTimeOffset>> selector)
		{
			return PropertyWithCustomDeserializer<DateTimeOffset, DeserializerConfigurationDateTimeOffset<TRecord>>(selector, propConf => new DeserializerConfigurationDateTimeOffset<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, byte[]?, DeserializerConfigurationByteArray<TRecord>> Property(Expression<Func<TRecord, byte[]?>> selector)
		{
			return PropertyWithCustomDeserializer<byte[]?, DeserializerConfigurationByteArray<TRecord>>(selector, propConf => new DeserializerConfigurationByteArray<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, bool?, DeserializerConfigurationBoolNullable<TRecord>> Property(Expression<Func<TRecord, bool?>> selector)
		{
			return PropertyWithCustomDeserializer<bool?, DeserializerConfigurationBoolNullable<TRecord>>(selector, propConf => new DeserializerConfigurationBoolNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, byte?, DeserializerConfigurationByteNullable<TRecord>> Property(Expression<Func<TRecord, byte?>> selector)
		{
			return PropertyWithCustomDeserializer<byte?, DeserializerConfigurationByteNullable<TRecord>>(selector, propConf => new DeserializerConfigurationByteNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, sbyte?, DeserializerConfigurationSByteNullable<TRecord>> Property(Expression<Func<TRecord, sbyte?>> selector)
		{
			return PropertyWithCustomDeserializer<sbyte?, DeserializerConfigurationSByteNullable<TRecord>>(selector, propConf => new DeserializerConfigurationSByteNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, short?, DeserializerConfigurationShortNullable<TRecord>> Property(Expression<Func<TRecord, short?>> selector)
		{
			return PropertyWithCustomDeserializer<short?, DeserializerConfigurationShortNullable<TRecord>>(selector, propConf => new DeserializerConfigurationShortNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, ushort?, DeserializerConfigurationUShortNullable<TRecord>> Property(Expression<Func<TRecord, ushort?>> selector)
		{
			return PropertyWithCustomDeserializer<ushort?, DeserializerConfigurationUShortNullable<TRecord>>(selector, propConf => new DeserializerConfigurationUShortNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, int?, DeserializerConfigurationIntNullable<TRecord>> Property(Expression<Func<TRecord, int?>> selector)
		{
			return PropertyWithCustomDeserializer<int?, DeserializerConfigurationIntNullable<TRecord>>(selector, propConf => new DeserializerConfigurationIntNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, uint?, DeserializerConfigurationUIntNullable<TRecord>> Property(Expression<Func<TRecord, uint?>> selector)
		{
			return PropertyWithCustomDeserializer<uint?, DeserializerConfigurationUIntNullable<TRecord>>(selector, propConf => new DeserializerConfigurationUIntNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, long?, DeserializerConfigurationLongNullable<TRecord>> Property(Expression<Func<TRecord, long?>> selector)
		{
			return PropertyWithCustomDeserializer<long?, DeserializerConfigurationLongNullable<TRecord>>(selector, propConf => new DeserializerConfigurationLongNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, ulong?, DeserializerConfigurationULongNullable<TRecord>> Property(Expression<Func<TRecord, ulong?>> selector)
		{
			return PropertyWithCustomDeserializer<ulong?, DeserializerConfigurationULongNullable<TRecord>>(selector, propConf => new DeserializerConfigurationULongNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, float?, DeserializerConfigurationFloatNullable<TRecord>> Property(Expression<Func<TRecord, float?>> selector)
		{
			return PropertyWithCustomDeserializer<float?, DeserializerConfigurationFloatNullable<TRecord>>(selector, propConf => new DeserializerConfigurationFloatNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, double?, DeserializerConfigurationDoubleNullable<TRecord>> Property(Expression<Func<TRecord, double?>> selector)
		{
			return PropertyWithCustomDeserializer<double?, DeserializerConfigurationDoubleNullable<TRecord>>(selector, propConf => new DeserializerConfigurationDoubleNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, decimal?, DeserializerConfigurationDecimalNullable<TRecord>> Property(Expression<Func<TRecord, decimal?>> selector)
		{
			return PropertyWithCustomDeserializer<decimal?, DeserializerConfigurationDecimalNullable<TRecord>>(selector, propConf => new DeserializerConfigurationDecimalNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, DateTime?, DeserializerConfigurationDateTimeNullable<TRecord>> Property(Expression<Func<TRecord, DateTime?>> selector)
		{
			return PropertyWithCustomDeserializer<DateTime?, DeserializerConfigurationDateTimeNullable<TRecord>>(selector, propConf => new DeserializerConfigurationDateTimeNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, TimeSpan?, DeserializerConfigurationTimeSpanNullable<TRecord>> Property(Expression<Func<TRecord, TimeSpan?>> selector)
		{
			return PropertyWithCustomDeserializer<TimeSpan?, DeserializerConfigurationTimeSpanNullable<TRecord>>(selector, propConf => new DeserializerConfigurationTimeSpanNullable<TRecord>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, DateTimeOffset?, DeserializerConfigurationDateTimeOffsetNullable<TRecord>> Property(Expression<Func<TRecord, DateTimeOffset?>> selector)
		{
			return PropertyWithCustomDeserializer<DateTimeOffset?, DeserializerConfigurationDateTimeOffsetNullable<TRecord>>(selector, propConf => new DeserializerConfigurationDateTimeOffsetNullable<TRecord>(propConf));
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

		public PropertyConfigurationFixedSerializer<TRecord, TEnum, DeserializerConfigurationEnum<TRecord, TEnum>> PropertyEnum<TEnum>(Expression<Func<TRecord, TEnum>> selector)
			where TEnum : struct
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
			return PropertyWithCustomDeserializer<TEnum, DeserializerConfigurationEnum<TRecord, TEnum>>(selector, propConf => new DeserializerConfigurationEnum<TRecord, TEnum>(propConf));
		}

		public PropertyConfigurationFixedSerializer<TRecord, TEnum?, DeserializerConfigurationEnumNullable<TRecord, TEnum>> PropertyEnum<TEnum>(Expression<Func<TRecord, TEnum?>> selector)
			where TEnum : struct
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
			return PropertyWithCustomDeserializer<TEnum?, DeserializerConfigurationEnumNullable<TRecord, TEnum>>(selector, propConf => new DeserializerConfigurationEnumNullable<TRecord, TEnum>(propConf));
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
	}
}