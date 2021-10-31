using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Deserializer configurator for nullable enum types
	/// </summary>
	/// <typeparam name="TEnum">Type of enum</typeparam>
	public class DeserializerConfigurationEnumNullable<TEnum> : DeserializerConfigurationBase<TEnum?>
		where TEnum : struct
	{
		private bool? _IgnoreCase;
		/// <summary>
		/// Is casing ignoring during enum parsing from text representation
		/// </summary>
		public bool IgnoreCase { get => _IgnoreCase??RecordConfiguration.DefaultEnumsIgnoreCase; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationEnumNullable(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{typeof(TEnum)} is not an Enum type");
		}

		/// <summary>
		/// Sets ignore case behavior during enum parsing from text representation
		/// </summary>
		/// <param name="ignoreCase">True to ignore case, false otherwise</param>
		/// <returns>This configuration object for methods chaining</returns>
		public DeserializerConfigurationEnumNullable<TEnum> SetIgnoreCase(bool ignoreCase)
		{
			_IgnoreCase = ignoreCase;
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<TEnum?>? cellDeserializer)
		{
			cellDeserializer = new CellEnumNullableDeserializer<TEnum>(IgnoreCase);
			return true;
		}
	}
}