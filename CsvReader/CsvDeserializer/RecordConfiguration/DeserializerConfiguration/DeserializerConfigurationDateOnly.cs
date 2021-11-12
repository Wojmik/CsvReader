using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration;

/// <summary>
/// Deserializer configurator for <see cref="DateOnly"/> type
/// </summary>
public class DeserializerConfigurationDateOnly : DeserializerConfigurationDateTimeStyleFormatProviderBase<DateOnly, DeserializerConfigurationDateOnly>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="bindingConfiguration">Binding to column configuration object</param>
	public DeserializerConfigurationDateOnly(BindingConfigurationBase bindingConfiguration)
		: base(bindingConfiguration)
	{ }

	internal override bool TryBuild([NotNullWhen(true)] out CellDeserializerBase<DateOnly>? cellDeserializer)
	{
		if (Format != null)
			cellDeserializer = new CellDateOnlyFormattedDeserializer(Format, FormatProvider, DateTimeStyles, AllowEmpty, ValueForEmpty);
		else
			cellDeserializer = new CellDateOnlyDeserializer(FormatProvider, DateTimeStyles, AllowEmpty, ValueForEmpty);
		return true;
	}
}
#endif