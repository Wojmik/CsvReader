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
	/// Deserializer configurator for <see cref="DateTimeOffset"/> type
	/// </summary>
	public class DeserializerConfigurationDateTimeOffset : DeserializerConfigurationDateTimeStyleFormatProviderBase<DateTimeOffset, DeserializerConfigurationDateTimeOffset>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationDateTimeOffset(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<DateTimeOffset>? cellDeserializer)
		{
			if(Format!=null)
				cellDeserializer = new CellDateTimeOffsetFormattedDeserializer(Format, FormatProvider, DateTimeStyles, AllowEmpty, ValueForEmpty);
			else
				cellDeserializer = new CellDateTimeOffsetDeserializer(FormatProvider, DateTimeStyles, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}