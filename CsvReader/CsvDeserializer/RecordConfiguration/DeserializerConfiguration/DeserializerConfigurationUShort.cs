﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Deserializer configurator for <see cref="ushort"/> type
	/// </summary>
	public class DeserializerConfigurationUShort : DeserializerConfigurationNumberStyleFormatProviderBase<ushort, DeserializerConfigurationUShort>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationUShort(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration, RecordConfigurationNumberStylesChooser.IntegerNumberStyles)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<ushort>? cellDeserializer)
		{
			cellDeserializer = new CellUShortDeserializer(NumberStyles, FormatProvider, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}