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
	/// Deserializer configurator for nullable <see cref="bool"/> type
	/// </summary>
	public class DeserializerConfigurationBoolNullable : DeserializerConfigurationBase<bool?>
	{
		private string? _trueString;
		/// <summary>
		/// Csv cell value that is interpreted as true value
		/// </summary>
		public string TrueString { get => _trueString??RecordConfiguration.DefaultBoolTrueString; }

		private string? _falseString;
		/// <summary>
		/// Csv cell value that is interpreted as false value
		/// </summary>
		public string FalseString { get => _falseString??RecordConfiguration.DefaultBoolFalseString; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationBoolNullable(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{ }

		/// <summary>
		/// Sets string that will be interpreted as true value
		/// </summary>
		/// <param name="trueString">Csv cell value that will be interpreted as true value</param>
		/// <returns>This configuration object for methods chaining</returns>
		public DeserializerConfigurationBoolNullable SetTrueString(string trueString)
		{
			_trueString = trueString??throw new ArgumentNullException(nameof(trueString));
			return this;
		}

		/// <summary>
		/// Sets string that will be interpreted as false value
		/// </summary>
		/// <param name="falseString">Csv cell value that will be interpreted as false value</param>
		/// <returns>This configuration object for methods chaining</returns>
		public DeserializerConfigurationBoolNullable SetFalseString(string falseString)
		{
			_falseString = falseString??throw new ArgumentNullException(nameof(falseString));
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<bool?>? cellDeserializer)
		{
			cellDeserializer = new CellBoolNullableDeserializer(TrueString, FalseString);
			return true;
		}
	}
}