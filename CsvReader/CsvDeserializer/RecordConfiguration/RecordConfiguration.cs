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
	/// <summary>
	/// Base class for configuring csv deserialization
	/// </summary>
	public abstract class RecordConfiguration
	{
		/// <summary>
		/// Has csv file header row
		/// </summary>
		public bool HasHeaderRow { get; }

		/// <summary>
		/// Default <see cref="IFormatProvider"/> for parsing types
		/// </summary>
		public IFormatProvider DefaultCulture { get; protected set; }

		/// <summary>
		/// Treat empty csv cells as nulls
		/// </summary>
		public bool DefaultEmptyAsNull { get; protected set; }

		/// <summary>
		/// Enables string deduplication feature. Equal string values are deduplicated to single string instance.
		/// </summary>
		public bool DefaultDeduplicateStrings { get; protected set; }

		/// <summary>
		/// Default byte array encoding
		/// </summary>
		public ByteArrayEncoding DefaultByteArrayEncoding { get; protected set; }

		/// <summary>
		/// Ignore case while parsing enums from text representations
		/// </summary>
		public bool DefaultEnumsIgnoreCase { get; protected set; }

		/// <summary>
		/// Default number styles for integer numbers
		/// </summary>
		public NumberStyles DefaultIntegerNumberStyles { get; protected set; }

		/// <summary>
		/// Default number styles for floating point numbers
		/// </summary>
		public NumberStyles DefaultFloationgPointNumberStyles { get; protected set; }

		/// <summary>
		/// Default number styles for <see cref="decimal"/> numbers
		/// </summary>
		public NumberStyles DefaultDecimalNumberStyles { get; protected set; }

		/// <summary>
		/// Default date styles
		/// </summary>
		public DateTimeStyles DefaultDateStyles { get; protected set; }

		/// <summary>
		/// Default bool true string
		/// </summary>
		public string DefaultBoolTrueString { get; protected set; }

		/// <summary>
		/// Default bool false string
		/// </summary>
		public string DefaultBoolFalseString { get; protected set; }

		/// <summary>
		/// String deduplicator
		/// </summary>
		public StringDeduplicator StringDeduplicator { get; }

		/// <summary>
		/// Is configuration object in building phase
		/// </summary>
		protected bool Building;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="options">Deserializer options</param>
		/// <param name="stringDeduplicator">String deduplicator</param>
		protected RecordConfiguration(CsvDeserializerOptions options, StringDeduplicator stringDeduplicator)
		{
			DefaultCulture = options.DeserializationCulture;
			HasHeaderRow = options.HasHeaderRow;
			DefaultEmptyAsNull = options.EmptyAsNull;
			DefaultDeduplicateStrings = options.DeduplicateStrings;
			StringDeduplicator = stringDeduplicator;
			DefaultByteArrayEncoding = ByteArrayEncoding.Base64;
			DefaultEnumsIgnoreCase = true;
			DefaultIntegerNumberStyles = NumberStyles.Integer;
			DefaultFloationgPointNumberStyles = NumberStyles.AllowThousands|NumberStyles.Float;
			DefaultDecimalNumberStyles = NumberStyles.Number;
			DefaultDateStyles = DateTimeStyles.None;
			DefaultBoolTrueString = bool.TrueString;
			DefaultBoolFalseString = bool.FalseString;
		}

		/// <summary>
		/// Method throws exception when configuration object is in building state
		/// </summary>
		/// <exception cref="InvalidOperationException">Object is in building state</exception>
		protected void CheckBuildingState()
		{
			if(Building)
				throw new InvalidOperationException("Cannot change configuration while building phase");
		}
	}

	/// <summary>
	/// Class for configuring csv deserialization to <typeparamref name="TRecord"/> type
	/// </summary>
	/// <typeparam name="TRecord">Type of records read from csv</typeparam>
	public class RecordConfiguration<TRecord> : RecordConfiguration
	{
		private ChangeParameterNameExpressionVisitor ChangeParameterNameExpressionVisitor { get; }

		private Dictionary<string, BindingConfigurationBase<TRecord>> PropertyConfigurations { get; }

		internal RecordConfiguration(CsvDeserializerOptions options, StringDeduplicator stringDeduplicator)
			: base(options, stringDeduplicator)
		{
			ChangeParameterNameExpressionVisitor = new ChangeParameterNameExpressionVisitor("0prm0");
			PropertyConfigurations = new Dictionary<string, BindingConfigurationBase<TRecord>>(StringComparer.Ordinal);
		}

		/// <summary>
		/// Sets default <see cref="IFormatProvider"/> for parsing types
		/// </summary>
		/// <param name="defaultCulture"><see cref="IFormatProvider"/> for parsing types</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		/// <exception cref="ArgumentNullException"><paramref name="defaultCulture"/> is null</exception>
		public RecordConfiguration<TRecord> SetDefaultCulture(IFormatProvider defaultCulture)
		{
			CheckBuildingState();
			DefaultCulture = defaultCulture??throw new ArgumentNullException(nameof(defaultCulture));
			return this;
		}

		/// <summary>
		/// Sets default empty csv cell behavior
		/// </summary>
		/// <param name="emptyAsNull">True to return nulls for empty csv cells, false to return empty objects</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		public RecordConfiguration<TRecord> SetDefaultEmptyBehavior(bool emptyAsNull)
		{
			CheckBuildingState();
			DefaultEmptyAsNull = emptyAsNull;
			return this;
		}

		/// <summary>
		/// Sets default duplicated strings behavior
		/// </summary>
		/// <param name="deduplicateStrings">True to turn on equal strings deduplication, false to turn it off</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		public RecordConfiguration<TRecord> SetDefaultStringsDeduplicationgBehavior(bool deduplicateStrings)
		{
			CheckBuildingState();
			DefaultDeduplicateStrings = deduplicateStrings;
			return this;
		}

		/// <summary>
		/// Sets default byte arrays encoding
		/// </summary>
		/// <param name="byteArrayEncoding">Default byte arrays encoding to set</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		public RecordConfiguration<TRecord> SetDefaultByteArrayEncoding(ByteArrayEncoding byteArrayEncoding)
		{
			CheckBuildingState();
			DefaultByteArrayEncoding = byteArrayEncoding;
			return this;
		}

		/// <summary>
		/// Sets default case sensitiveness while parsing enums from text representations
		/// </summary>
		/// <param name="enumsIgnoreCase">True to ignore case while parsing enums, false for exact matching</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		public RecordConfiguration<TRecord> SetDefaultEnumsIgnoreCaseBehavior(bool enumsIgnoreCase)
		{
			CheckBuildingState();
			DefaultEnumsIgnoreCase = enumsIgnoreCase;
			return this;
		}

		/// <summary>
		/// Sets default number styles for integer numbers
		/// </summary>
		/// <param name="defaultNumberStyles">Number styles to set as default</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		public RecordConfiguration<TRecord> SetDefaultIntegerNumberStyles(NumberStyles defaultNumberStyles)
		{
			CheckBuildingState();
			DefaultIntegerNumberStyles = defaultNumberStyles;
			return this;
		}

		/// <summary>
		/// Sets default number styles for floating point numbers
		/// </summary>
		/// <param name="defaultNumberStyles">Number styles to set as default</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		public RecordConfiguration<TRecord> SetDefaultFloationgPointNumberStyles(NumberStyles defaultNumberStyles)
		{
			CheckBuildingState();
			DefaultFloationgPointNumberStyles = defaultNumberStyles;
			return this;
		}

		/// <summary>
		/// Sets default number styles for <see cref="decimal"/> numbers
		/// </summary>
		/// <param name="defaultNumberStyles">Number styles to set as default</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		public RecordConfiguration<TRecord> SetDefaultDecimalNumberStyles(NumberStyles defaultNumberStyles)
		{
			CheckBuildingState();
			DefaultDecimalNumberStyles = defaultNumberStyles;
			return this;
		}

		/// <summary>
		/// Sets default date styles
		/// </summary>
		/// <param name="defaultDateStyles">Date styles to set as default</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		public RecordConfiguration<TRecord> SetDefaultDecimalNumberStyles(DateTimeStyles defaultDateStyles)
		{
			CheckBuildingState();
			DefaultDateStyles = defaultDateStyles;
			return this;
		}

		/// <summary>
		/// Sets default bool true string
		/// </summary>
		/// <param name="defaultTrueString">String to set as default true string</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		/// <exception cref="ArgumentNullException"><paramref name="defaultTrueString"/> is null</exception>
		public RecordConfiguration<TRecord> SetDefaultBoolTrueString(string defaultTrueString)
		{
			CheckBuildingState();
			DefaultBoolTrueString = defaultTrueString??throw new ArgumentNullException(nameof(defaultTrueString));
			return this;
		}

		/// <summary>
		/// Sets default bool false string
		/// </summary>
		/// <param name="defaultFalseString">String to set as default false string</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="InvalidOperationException">Object is in building state, configuration cannot be changed anymore</exception>
		/// <exception cref="ArgumentNullException"><paramref name="defaultFalseString"/> is null</exception>
		public RecordConfiguration<TRecord> SetDefaultBoolFalseString(string defaultFalseString)
		{
			CheckBuildingState();
			DefaultBoolFalseString = defaultFalseString??throw new ArgumentNullException(nameof(defaultFalseString));
			return this;
		}

		internal IEnumerable<ColumnBinding<TRecord>> Build()
		{
			Building = true;

			foreach(var property in PropertyConfigurations)
				if(property.Value.TryBuild(out var binding))
					yield return binding!;
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

		/// <summary>
		/// Clears all binding configurations. After call this method no record's property would be deserialized. Bindings have to be set manually for record's properties to be deserialized.
		/// </summary>
		/// <returns>This configuration object for methods chaining</returns>
		public RecordConfiguration<TRecord> ClearAllBindings()
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
			Type? underlyingType;

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
			else if(propertyType==typeof(Guid))
				bindingConfigurationBase = Property((Expression<Func<TRecord, Guid>>)selectorLambdaExpression);
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
			else if(propertyType==typeof(Guid?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, Guid?>>)selectorLambdaExpression);
#if NET5_0_OR_GREATER
			else if(propertyType==typeof(Half))
				bindingConfigurationBase = Property((Expression<Func<TRecord, Half>>)selectorLambdaExpression);
			else if(propertyType==typeof(Half?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, Half?>>)selectorLambdaExpression);
#endif
#if NET6_0_OR_GREATER
			else if (propertyType == typeof(DateOnly))
				bindingConfigurationBase = Property((Expression<Func<TRecord, DateOnly>>)selectorLambdaExpression);
			else if (propertyType == typeof(TimeOnly))
				bindingConfigurationBase = Property((Expression<Func<TRecord, TimeOnly>>)selectorLambdaExpression);
			else if (propertyType == typeof(DateOnly?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, DateOnly?>>)selectorLambdaExpression);
			else if (propertyType == typeof(TimeOnly?))
				bindingConfigurationBase = Property((Expression<Func<TRecord, TimeOnly?>>)selectorLambdaExpression);
#endif
			else if(propertyType.IsEnum)
			{
				Func<Expression<Func<TRecord, int>>, PropertyConfigurationFixedSerializer<TRecord, int, DeserializerConfigurationEnum<int>>> func = PropertyEnum;
				var genericMethodDefinition = func.Method.GetGenericMethodDefinition();
				var methodInfo = genericMethodDefinition.MakeGenericMethod(propertyType);
				var restult = methodInfo.Invoke(this, new object[] { selectorLambdaExpression, })!;
				bindingConfigurationBase = (BindingConfigurationBase<TRecord>)restult;
			}
			else if((underlyingType=Nullable.GetUnderlyingType(propertyType))!=null && underlyingType.IsEnum)
			{
				Func<Expression<Func<TRecord, int?>>, PropertyConfigurationFixedSerializer<TRecord, int?, DeserializerConfigurationEnumNullable<int>>> func = PropertyEnum;
				var genericMethodDefinition = func.Method.GetGenericMethodDefinition();
				var methodInfo = genericMethodDefinition.MakeGenericMethod(underlyingType);
				var restult = methodInfo.Invoke(this, new object[] { selectorLambdaExpression, })!;
				bindingConfigurationBase = (BindingConfigurationBase<TRecord>)restult;
			}

			if(HasHeaderRow && bindingConfigurationBase!=null)
				bindingConfigurationBase.BindToColumnInternal(propertyInfo.Name);
		}
		#endregion
		#region Property
		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, string?, DeserializerConfigurationString> Property(Expression<Func<TRecord, string?>> selector)
		{
			return PropertyWithCustomDeserializer<string?, DeserializerConfigurationString>(selector, propConf => new DeserializerConfigurationString(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, bool, DeserializerConfigurationBool> Property(Expression<Func<TRecord, bool>> selector)
		{
			return PropertyWithCustomDeserializer<bool, DeserializerConfigurationBool>(selector, propConf => new DeserializerConfigurationBool(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, byte, DeserializerConfigurationByte> Property(Expression<Func<TRecord, byte>> selector)
		{
			return PropertyWithCustomDeserializer<byte, DeserializerConfigurationByte>(selector, propConf => new DeserializerConfigurationByte(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, sbyte, DeserializerConfigurationSByte> Property(Expression<Func<TRecord, sbyte>> selector)
		{
			return PropertyWithCustomDeserializer<sbyte, DeserializerConfigurationSByte>(selector, propConf => new DeserializerConfigurationSByte(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, short, DeserializerConfigurationShort> Property(Expression<Func<TRecord, short>> selector)
		{
			return PropertyWithCustomDeserializer<short, DeserializerConfigurationShort>(selector, propConf => new DeserializerConfigurationShort(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, ushort, DeserializerConfigurationUShort> Property(Expression<Func<TRecord, ushort>> selector)
		{
			return PropertyWithCustomDeserializer<ushort, DeserializerConfigurationUShort>(selector, propConf => new DeserializerConfigurationUShort(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, int, DeserializerConfigurationInt> Property(Expression<Func<TRecord, int>> selector)
		{
			return PropertyWithCustomDeserializer<int, DeserializerConfigurationInt>(selector, propConf => new DeserializerConfigurationInt(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, uint, DeserializerConfigurationUInt> Property(Expression<Func<TRecord, uint>> selector)
		{
			return PropertyWithCustomDeserializer<uint, DeserializerConfigurationUInt>(selector, propConf => new DeserializerConfigurationUInt(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, long, DeserializerConfigurationLong> Property(Expression<Func<TRecord, long>> selector)
		{
			return PropertyWithCustomDeserializer<long, DeserializerConfigurationLong>(selector, propConf => new DeserializerConfigurationLong(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, ulong, DeserializerConfigurationULong> Property(Expression<Func<TRecord, ulong>> selector)
		{
			return PropertyWithCustomDeserializer<ulong, DeserializerConfigurationULong>(selector, propConf => new DeserializerConfigurationULong(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, float, DeserializerConfigurationFloat> Property(Expression<Func<TRecord, float>> selector)
		{
			return PropertyWithCustomDeserializer<float, DeserializerConfigurationFloat>(selector, propConf => new DeserializerConfigurationFloat(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, double, DeserializerConfigurationDouble> Property(Expression<Func<TRecord, double>> selector)
		{
			return PropertyWithCustomDeserializer<double, DeserializerConfigurationDouble>(selector, propConf => new DeserializerConfigurationDouble(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, decimal, DeserializerConfigurationDecimal> Property(Expression<Func<TRecord, decimal>> selector)
		{
			return PropertyWithCustomDeserializer<decimal, DeserializerConfigurationDecimal>(selector, propConf => new DeserializerConfigurationDecimal(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, DateTime, DeserializerConfigurationDateTime> Property(Expression<Func<TRecord, DateTime>> selector)
		{
			return PropertyWithCustomDeserializer<DateTime, DeserializerConfigurationDateTime>(selector, propConf => new DeserializerConfigurationDateTime(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TimeSpan, DeserializerConfigurationTimeSpan> Property(Expression<Func<TRecord, TimeSpan>> selector)
		{
			return PropertyWithCustomDeserializer<TimeSpan, DeserializerConfigurationTimeSpan>(selector, propConf => new DeserializerConfigurationTimeSpan(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, DateTimeOffset, DeserializerConfigurationDateTimeOffset> Property(Expression<Func<TRecord, DateTimeOffset>> selector)
		{
			return PropertyWithCustomDeserializer<DateTimeOffset, DeserializerConfigurationDateTimeOffset>(selector, propConf => new DeserializerConfigurationDateTimeOffset(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, Guid, DeserializerConfigurationGuid> Property(Expression<Func<TRecord, Guid>> selector)
		{
			return PropertyWithCustomDeserializer<Guid, DeserializerConfigurationGuid>(selector, propConf => new DeserializerConfigurationGuid(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, byte[]?, DeserializerConfigurationByteArray> Property(Expression<Func<TRecord, byte[]?>> selector)
		{
			return PropertyWithCustomDeserializer<byte[]?, DeserializerConfigurationByteArray>(selector, propConf => new DeserializerConfigurationByteArray(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, bool?, DeserializerConfigurationBoolNullable> Property(Expression<Func<TRecord, bool?>> selector)
		{
			return PropertyWithCustomDeserializer<bool?, DeserializerConfigurationBoolNullable>(selector, propConf => new DeserializerConfigurationBoolNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, byte?, DeserializerConfigurationByteNullable> Property(Expression<Func<TRecord, byte?>> selector)
		{
			return PropertyWithCustomDeserializer<byte?, DeserializerConfigurationByteNullable>(selector, propConf => new DeserializerConfigurationByteNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, sbyte?, DeserializerConfigurationSByteNullable> Property(Expression<Func<TRecord, sbyte?>> selector)
		{
			return PropertyWithCustomDeserializer<sbyte?, DeserializerConfigurationSByteNullable>(selector, propConf => new DeserializerConfigurationSByteNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, short?, DeserializerConfigurationShortNullable> Property(Expression<Func<TRecord, short?>> selector)
		{
			return PropertyWithCustomDeserializer<short?, DeserializerConfigurationShortNullable>(selector, propConf => new DeserializerConfigurationShortNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, ushort?, DeserializerConfigurationUShortNullable> Property(Expression<Func<TRecord, ushort?>> selector)
		{
			return PropertyWithCustomDeserializer<ushort?, DeserializerConfigurationUShortNullable>(selector, propConf => new DeserializerConfigurationUShortNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, int?, DeserializerConfigurationIntNullable> Property(Expression<Func<TRecord, int?>> selector)
		{
			return PropertyWithCustomDeserializer<int?, DeserializerConfigurationIntNullable>(selector, propConf => new DeserializerConfigurationIntNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, uint?, DeserializerConfigurationUIntNullable> Property(Expression<Func<TRecord, uint?>> selector)
		{
			return PropertyWithCustomDeserializer<uint?, DeserializerConfigurationUIntNullable>(selector, propConf => new DeserializerConfigurationUIntNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, long?, DeserializerConfigurationLongNullable> Property(Expression<Func<TRecord, long?>> selector)
		{
			return PropertyWithCustomDeserializer<long?, DeserializerConfigurationLongNullable>(selector, propConf => new DeserializerConfigurationLongNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, ulong?, DeserializerConfigurationULongNullable> Property(Expression<Func<TRecord, ulong?>> selector)
		{
			return PropertyWithCustomDeserializer<ulong?, DeserializerConfigurationULongNullable>(selector, propConf => new DeserializerConfigurationULongNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, float?, DeserializerConfigurationFloatNullable> Property(Expression<Func<TRecord, float?>> selector)
		{
			return PropertyWithCustomDeserializer<float?, DeserializerConfigurationFloatNullable>(selector, propConf => new DeserializerConfigurationFloatNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, double?, DeserializerConfigurationDoubleNullable> Property(Expression<Func<TRecord, double?>> selector)
		{
			return PropertyWithCustomDeserializer<double?, DeserializerConfigurationDoubleNullable>(selector, propConf => new DeserializerConfigurationDoubleNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, decimal?, DeserializerConfigurationDecimalNullable> Property(Expression<Func<TRecord, decimal?>> selector)
		{
			return PropertyWithCustomDeserializer<decimal?, DeserializerConfigurationDecimalNullable>(selector, propConf => new DeserializerConfigurationDecimalNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, DateTime?, DeserializerConfigurationDateTimeNullable> Property(Expression<Func<TRecord, DateTime?>> selector)
		{
			return PropertyWithCustomDeserializer<DateTime?, DeserializerConfigurationDateTimeNullable>(selector, propConf => new DeserializerConfigurationDateTimeNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TimeSpan?, DeserializerConfigurationTimeSpanNullable> Property(Expression<Func<TRecord, TimeSpan?>> selector)
		{
			return PropertyWithCustomDeserializer<TimeSpan?, DeserializerConfigurationTimeSpanNullable>(selector, propConf => new DeserializerConfigurationTimeSpanNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, DateTimeOffset?, DeserializerConfigurationDateTimeOffsetNullable> Property(Expression<Func<TRecord, DateTimeOffset?>> selector)
		{
			return PropertyWithCustomDeserializer<DateTimeOffset?, DeserializerConfigurationDateTimeOffsetNullable>(selector, propConf => new DeserializerConfigurationDateTimeOffsetNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, Guid?, DeserializerConfigurationGuidNullable> Property(Expression<Func<TRecord, Guid?>> selector)
		{
			return PropertyWithCustomDeserializer<Guid?, DeserializerConfigurationGuidNullable>(selector, propConf => new DeserializerConfigurationGuidNullable(propConf));
		}

#if NET5_0_OR_GREATER
		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, Half, DeserializerConfigurationHalf> Property(Expression<Func<TRecord, Half>> selector)
		{
			return PropertyWithCustomDeserializer<Half, DeserializerConfigurationHalf>(selector, propConf => new DeserializerConfigurationHalf(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, Half?, DeserializerConfigurationHalfNullable> Property(Expression<Func<TRecord, Half?>> selector)
		{
			return PropertyWithCustomDeserializer<Half?, DeserializerConfigurationHalfNullable>(selector, propConf => new DeserializerConfigurationHalfNullable(propConf));
		}
#endif
#if NET6_0_OR_GREATER
		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, DateOnly, DeserializerConfigurationDateOnly> Property(Expression<Func<TRecord, DateOnly>> selector)
		{
			return PropertyWithCustomDeserializer<DateOnly, DeserializerConfigurationDateOnly>(selector, propConf => new DeserializerConfigurationDateOnly(propConf));
		}
		
		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TimeOnly, DeserializerConfigurationTimeOnly> Property(Expression<Func<TRecord, TimeOnly>> selector)
		{
			return PropertyWithCustomDeserializer<TimeOnly, DeserializerConfigurationTimeOnly>(selector, propConf => new DeserializerConfigurationTimeOnly(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, DateOnly?, DeserializerConfigurationDateOnlyNullable> Property(Expression<Func<TRecord, DateOnly?>> selector)
		{
			return PropertyWithCustomDeserializer<DateOnly?, DeserializerConfigurationDateOnlyNullable>(selector, propConf => new DeserializerConfigurationDateOnlyNullable(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TimeOnly?, DeserializerConfigurationTimeOnlyNullable> Property(Expression<Func<TRecord, TimeOnly?>> selector)
		{
			return PropertyWithCustomDeserializer<TimeOnly?, DeserializerConfigurationTimeOnlyNullable>(selector, propConf => new DeserializerConfigurationTimeOnlyNullable(propConf));
		}
#endif

		/// <summary>
		/// Enables configuring of selected record's property
		/// </summary>
		/// <typeparam name="TProperty">Type of record's property</typeparam>
		/// <param name="selector">Record's property selector</param>
		/// <returns>Configuration object for selected property</returns>
		/// <exception cref="InvalidOperationException">Serializer for selected property is not defined. Use one of PropertyWithCustomDeserializer method to cereate one</exception>
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

		/// <summary>
		/// Enables configuring of selected record's enum property
		/// </summary>
		/// <typeparam name="TEnum">Type of enum</typeparam>
		/// <param name="selector">Record's enum property selector</param>
		/// <returns>Configuration object for selected enum property</returns>
		/// <exception cref="ArgumentException"><typeparamref name="TEnum"/> is not enum type</exception>
		public PropertyConfigurationFixedSerializer<TRecord, TEnum, DeserializerConfigurationEnum<TEnum>> PropertyEnum<TEnum>(Expression<Func<TRecord, TEnum>> selector)
			where TEnum : struct
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
			return PropertyWithCustomDeserializer<TEnum, DeserializerConfigurationEnum<TEnum>>(selector, propConf => new DeserializerConfigurationEnum<TEnum>(propConf));
		}

		/// <summary>
		/// Enables configuring of selected record's enum property
		/// </summary>
		/// <typeparam name="TEnum">Type of enum</typeparam>
		/// <param name="selector">Record's enum property selector</param>
		/// <returns>Configuration object for selected enum property</returns>
		/// <exception cref="ArgumentException"><typeparamref name="TEnum"/> is not enum type</exception>
		public PropertyConfigurationFixedSerializer<TRecord, TEnum?, DeserializerConfigurationEnumNullable<TEnum>> PropertyEnum<TEnum>(Expression<Func<TRecord, TEnum?>> selector)
			where TEnum : struct
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
			return PropertyWithCustomDeserializer<TEnum?, DeserializerConfigurationEnumNullable<TEnum>>(selector, propConf => new DeserializerConfigurationEnumNullable<TEnum>(propConf));
		}
		#endregion
		#region PropertyWithCustomDeserializer
		/// <summary>
		/// Enables configuring of selected record's property with custom deserializer configurator
		/// </summary>
		/// <typeparam name="TProperty">Type of record's property</typeparam>
		/// <typeparam name="TDeserializerConfigurator">Type of deserializer configuration object</typeparam>
		/// <param name="selector">Record's property selector</param>
		/// <param name="createCustomDeserializerConfiguratorMethod">Delegate for creating custom deserializer configuration object</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator> PropertyWithCustomDeserializer<TProperty, TDeserializerConfigurator>(
			Expression<Func<TRecord, TProperty>> selector,
			Func<PropertyConfigurationFixedSerializer<TRecord, TProperty, TDeserializerConfigurator>, TDeserializerConfigurator> createCustomDeserializerConfiguratorMethod
			)
			where TDeserializerConfigurator : DeserializerConfigurationBase<TProperty>
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

		/// <summary>
		/// Enables configuring of selected record's property with custom deserializer
		/// </summary>
		/// <typeparam name="TProperty">Type of record's property</typeparam>
		/// <param name="selector">Record's property selector</param>
		/// <param name="deserializer">Deserialization object, from <see cref="MemorySequenceSpan"/> to <typeparamref name="TProperty"/> type</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, CellDeserializerFromMemorySequenceBase<TProperty> deserializer)
		{
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TProperty>(propConf, deserializer));
		}

		/// <summary>
		/// Enables configuring of selected record's property with custom deserializer
		/// </summary>
		/// <typeparam name="TProperty">Type of record's property</typeparam>
		/// <param name="selector">Record's property selector</param>
		/// <param name="deserializer">Deserialization object, from <see cref="ReadOnlyMemory{T}"/> to <typeparamref name="TProperty"/> type</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, CellDeserializerFromMemoryBase<TProperty> deserializer)
		{
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TProperty>(propConf, deserializer));
		}

		/// <summary>
		/// Enables configuring of selected record's property with custom deserializer
		/// </summary>
		/// <typeparam name="TProperty">Type of record's property</typeparam>
		/// <param name="selector">Record's property selector</param>
		/// <param name="deserializer">Deserialization object, from <see cref="string"/> to <typeparamref name="TProperty"/> type</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, CellDeserializerFromStringBase<TProperty> deserializer)
		{
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TProperty>(propConf, deserializer));
		}

		/// <summary>
		/// Enables configuring of selected record's property with custom deserialization method
		/// </summary>
		/// <typeparam name="TProperty">Type of record's property</typeparam>
		/// <param name="selector">Record's property selector</param>
		/// <param name="deserializeMethod">Deserialization method, from <see cref="MemorySequenceSpan"/> to <typeparamref name="TProperty"/> type</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, Func<MemorySequenceSpan, TProperty> deserializeMethod)
		{
			var deserializer = new CellDeserializerFromMemorySequence<TProperty>(deserializeMethod);
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TProperty>(propConf, deserializer));
		}

		/// <summary>
		/// Enables configuring of selected record's property with custom deserialization method
		/// </summary>
		/// <typeparam name="TProperty">Type of record's property</typeparam>
		/// <param name="selector">Record's property selector</param>
		/// <param name="deserializeMethod">Deserialization method, from <see cref="ReadOnlyMemory{T}"/> to <typeparamref name="TProperty"/> type</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, Func<ReadOnlyMemory<char>, TProperty> deserializeMethod)
		{
			var deserializer = new CellDeserializerFromMemory<TProperty>(deserializeMethod);
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TProperty>(propConf, deserializer));
		}

		/// <summary>
		/// Enables configuring of selected record's property with custom deserialization method
		/// </summary>
		/// <typeparam name="TProperty">Type of record's property</typeparam>
		/// <param name="selector">Record's property selector</param>
		/// <param name="deserializeMethod">Deserialization method, from <see cref="string"/> to <typeparamref name="TProperty"/> type</param>
		/// <returns>Configuration object for selected property</returns>
		public PropertyConfigurationFixedSerializer<TRecord, TProperty, DeserializerConfigurationCustom<TProperty>> PropertyWithCustomDeserializer<TProperty>(Expression<Func<TRecord, TProperty>> selector, Func<string, TProperty> deserializeMethod)
		{
			var deserializer = new CellDeserializerFromString<TProperty>(deserializeMethod);
			return PropertyWithCustomDeserializer<TProperty, DeserializerConfigurationCustom<TProperty>>(selector, propConf => new DeserializerConfigurationCustom<TProperty>(propConf, deserializer));
		}
		#endregion
	}
}