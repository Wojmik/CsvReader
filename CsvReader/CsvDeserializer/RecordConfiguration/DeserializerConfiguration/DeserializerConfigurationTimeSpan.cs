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
	/// Deserializer configurator for <see cref="TimeSpan"/> type
	/// </summary>
	public class DeserializerConfigurationTimeSpan : DeserializerConfigurationFormatProviderBase<TimeSpan, DeserializerConfigurationTimeSpan>
	{
		/// <summary>
		/// Format used during parsing cell value to target type. If null, standard formats are used.
		/// </summary>
		public string? Format { get; private set; }

		/// <summary>
		/// <see cref="TimeSpan"/> styles used during parsing cell value to a date
		/// </summary>
		public TimeSpanStyles TimeSpanStyles { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationTimeSpan(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{
			TimeSpanStyles = TimeSpanStyles.None;
		}

		/// <summary>
		/// Sets format and <see cref="TimeSpan"/> styles used during parsing cell value to <see cref="TimeSpan"/> type
		/// </summary>
		/// <param name="format">Desired format for parsing cell value to <see cref="TimeSpan"/> type</param>
		/// <param name="timeSpanStyles"><see cref="TimeSpan"/> styles used during parsing cell value</param>
		/// <returns>This configuration object for methods chaining</returns>
		/// <exception cref="ArgumentNullException"><paramref name="format"/> is null</exception>
		public DeserializerConfigurationTimeSpan SetFormatAndStyles(string format, TimeSpanStyles timeSpanStyles = TimeSpanStyles.None)
		{
			Format = format??throw new ArgumentNullException(nameof(format));
			TimeSpanStyles = timeSpanStyles;
			return this;
		}

		/// <summary>
		/// Clears format and sets <see cref="TimeSpan"/> styles to <see cref="TimeSpanStyles.None"/>. Those values will be used during parsing cell value to <see cref="TimeSpan"/> type.
		/// </summary>
		/// <returns>This configuration object for methods chaining</returns>
		public DeserializerConfigurationTimeSpan ClearFormatAndStyles()
		{
			Format = null;
			TimeSpanStyles = TimeSpanStyles.None;
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<TimeSpan>? cellDeserializer)
		{
			if(Format!=null)
				cellDeserializer = new CellTimeSpanFormattedDeserializer(Format, FormatProvider, TimeSpanStyles, AllowEmpty, ValueForEmpty);
			else
				cellDeserializer = new CellTimeSpanDeserializer(FormatProvider, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}