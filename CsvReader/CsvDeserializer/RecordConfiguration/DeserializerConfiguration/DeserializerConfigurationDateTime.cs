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
	/// Deserializer configurator for <see cref="DateTime"/> type
	/// </summary>
	public class DeserializerConfigurationDateTime : DeserializerConfigurationDateTimeStyleFormatProviderBase<DateTime, DeserializerConfigurationDateTime>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationDateTime(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{ }

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<DateTime>? cellDeserializer)
		{
			if(Format!=null)
				cellDeserializer = new CellDateTimeFormattedDeserializer(Format, FormatProvider, DateTimeStyles, AllowEmpty, ValueForEmpty);
			else
				cellDeserializer = new CellDateTimeDeserializer(FormatProvider, DateTimeStyles, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}