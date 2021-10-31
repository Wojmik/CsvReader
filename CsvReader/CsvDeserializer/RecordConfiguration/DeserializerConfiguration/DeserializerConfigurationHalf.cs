using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
#if NET5_0_OR_GREATER
	/// <summary>
	/// Deserializer configurator for <see cref="Half"/> type
	/// </summary>
	public class DeserializerConfigurationHalf : DeserializerConfigurationNumberStyleFormatProviderBase<Half, DeserializerConfigurationHalf>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationHalf(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration, RecordConfigurationNumberStylesChooser.FloatingPointNumberStyles)
		{ }

		internal override bool TryBuild([NotNullWhen(true)] out CellDeserializerBase<Half>? cellDeserializer)
		{
			cellDeserializer = new CellHalfDeserializer(NumberStyles, FormatProvider, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
#endif
}