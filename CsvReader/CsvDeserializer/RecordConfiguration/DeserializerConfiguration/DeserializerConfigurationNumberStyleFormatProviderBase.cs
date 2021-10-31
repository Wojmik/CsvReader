using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Base deserializer configurator for number types
	/// </summary>
	/// <typeparam name="TDeserialized">Type deserializer deserializes to</typeparam>
	/// <typeparam name="TDeserializerConfigurator">Type of deserializer configurator</typeparam>
	public abstract class DeserializerConfigurationNumberStyleFormatProviderBase<TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationNotNullableBase<TDeserialized, TDeserializerConfigurator>
		where TDeserialized : struct
		where TDeserializerConfigurator : DeserializerConfigurationNumberStyleFormatProviderBase<TDeserialized, TDeserializerConfigurator>
	{
		private readonly RecordConfigurationNumberStylesChooser RecordConfigurationNumberStylesChooser;

		private NumberStyles? _NumberStyles;
		/// <summary>
		/// Number styles used during parsing cell value to a number
		/// </summary>
		public NumberStyles NumberStyles
		{
			get
			{
				return _NumberStyles??RecordConfigurationNumberStylesChooser switch
				{
					RecordConfigurationNumberStylesChooser.IntegerNumberStyles => RecordConfiguration.DefaultIntegerNumberStyles,
					RecordConfigurationNumberStylesChooser.FloatingPointNumberStyles => RecordConfiguration.DefaultFloationgPointNumberStyles,
					RecordConfigurationNumberStylesChooser.DecimalNumberStyles => RecordConfiguration.DefaultDecimalNumberStyles,
					_ => throw new NotSupportedException($"Unsupported record configuration number styles chooser: {RecordConfigurationNumberStylesChooser}"),
				};
			}	
		}

		private IFormatProvider? _FormatProvider;
		/// <summary>
		/// Format provider used during parsing cell value to target type
		/// </summary>
		public IFormatProvider FormatProvider { get => _FormatProvider??RecordConfiguration.DefaultCulture; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		/// <param name="recordConfigurationNumberStylesChooser">Points out <see cref="WojciechMikołajewicz.CsvReader.RecordConfiguration"/> property of default number styles</param>
		/// <exception cref="NotSupportedException">Invalid value of <paramref name="recordConfigurationNumberStylesChooser"/></exception>
		protected DeserializerConfigurationNumberStyleFormatProviderBase(BindingConfigurationBase bindingConfiguration, RecordConfigurationNumberStylesChooser recordConfigurationNumberStylesChooser)
			: base(bindingConfiguration)
		{
			if(!Enum.IsDefined(typeof(RecordConfigurationNumberStylesChooser), recordConfigurationNumberStylesChooser))
				throw new NotSupportedException($"Unsupported record configuration number styles chooser: {recordConfigurationNumberStylesChooser}");
			RecordConfigurationNumberStylesChooser = recordConfigurationNumberStylesChooser;
		}

		/// <summary>
		/// Sets number styles used during parsing cell value to a number
		/// </summary>
		/// <param name="numberStyles">Desired number styles for parsing cell value to a number</param>
		/// <returns>This configuration object for methods chaining</returns>
		public TDeserializerConfigurator SetNumberStyles(NumberStyles numberStyles)
		{
			_NumberStyles = numberStyles;
			return (TDeserializerConfigurator)this;
		}

		/// <summary>
		/// Sets format provider used during parsing cell value to <typeparamref name="TDeserialized"/> type. If null, <see cref="WojciechMikołajewicz.CsvReader.RecordConfiguration.DefaultCulture"/> is used.
		/// </summary>
		/// <param name="formatProvider">Desired format provider for parsing cell value to <typeparamref name="TDeserialized"/> type</param>
		/// <returns>This configuration object for methods chaining</returns>
		public TDeserializerConfigurator SetFormatProvider(IFormatProvider? formatProvider)
		{
			_FormatProvider = formatProvider;
			return (TDeserializerConfigurator)this;
		}
	}
}