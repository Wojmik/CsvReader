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
	/// Deserializer configurator for <see cref="decimal"/> type
	/// </summary>
	public class DeserializerConfigurationDecimal : DeserializerConfigurationNumberStyleFormatProviderBase<decimal, DeserializerConfigurationDecimal>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationDecimal(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration, RecordConfigurationNumberStylesChooser.DecimalNumberStyles)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<decimal>? cellDeserializer)
		{
			cellDeserializer = new CellDecimalDeserializer(NumberStyles, FormatProvider, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}