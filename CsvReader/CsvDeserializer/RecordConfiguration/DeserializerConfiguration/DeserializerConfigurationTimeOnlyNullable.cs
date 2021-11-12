using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

#if NET6_0_OR_GREATER
namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration;

/// <summary>
/// Deserializer configurator for nullable <see cref="DateOnly"/> type
/// </summary>
public class DeserializerConfigurationTimeOnlyNullable : DeserializerConfigurationDateTimeStyleFormatProviderNullableBase<TimeOnly, DeserializerConfigurationTimeOnlyNullable>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="bindingConfiguration">Binding to column configuration object</param>
	public DeserializerConfigurationTimeOnlyNullable(BindingConfigurationBase bindingConfiguration)
		: base(bindingConfiguration)
	{ }

	internal override bool TryBuild([NotNullWhen(true)] out CellDeserializerBase<TimeOnly?>? cellDeserializer)
	{
		if (Format != null)
			cellDeserializer = new CellTimeOnlyFormattedNullableDeserializer(Format, FormatProvider, DateTimeStyles);
		else
			cellDeserializer = new CellTimeOnlyNullableDeserializer(FormatProvider, DateTimeStyles);
		return true;
	}
}
#endif