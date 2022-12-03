using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Base deserializer configurator for nullable number types
	/// </summary>
	/// <typeparam name="TDeserialized">Type deserializer deserializes to</typeparam>
	/// <typeparam name="TDeserializerConfigurator">Type of deserializer configurator</typeparam>
	public abstract class DeserializerConfigurationNumberStyleFormatProviderNullableBase<TDeserialized, TDeserializerConfigurator> : DeserializerConfigurationBase<TDeserialized?>
		where TDeserialized : struct
		where TDeserializerConfigurator : DeserializerConfigurationNumberStyleFormatProviderNullableBase<TDeserialized, TDeserializerConfigurator>
	{
		private readonly RecordConfigurationNumberStylesChooser _recordConfigurationNumberStylesChooser;

		private NumberStyles? _numberStyles;
		/// <summary>
		/// Number styles used during parsing cell value to a number
		/// </summary>
		public NumberStyles NumberStyles
		{
			get
			{
				return _numberStyles??_recordConfigurationNumberStylesChooser switch
				{
					RecordConfigurationNumberStylesChooser.IntegerNumberStyles => RecordConfiguration.DefaultIntegerNumberStyles,
					RecordConfigurationNumberStylesChooser.FloatingPointNumberStyles => RecordConfiguration.DefaultFloationgPointNumberStyles,
					RecordConfigurationNumberStylesChooser.DecimalNumberStyles => RecordConfiguration.DefaultDecimalNumberStyles,
					_ => throw new NotSupportedException($"Unsupported record configuration number styles chooser: {_recordConfigurationNumberStylesChooser}"),
				};
			}
		}

		private IFormatProvider? _formatProvider;
		/// <summary>
		/// Format provider used during parsing cell value to target type
		/// </summary>
		public IFormatProvider FormatProvider { get => _formatProvider??RecordConfiguration.DefaultCulture; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		/// <param name="recordConfigurationNumberStylesChooser">Points out <see cref="WojciechMikołajewicz.CsvReader.RecordConfiguration"/> property of default number styles</param>
		/// <exception cref="NotSupportedException">Invalid value of <paramref name="recordConfigurationNumberStylesChooser"/></exception>
		protected DeserializerConfigurationNumberStyleFormatProviderNullableBase(BindingConfigurationBase bindingConfiguration, RecordConfigurationNumberStylesChooser recordConfigurationNumberStylesChooser)
			: base(bindingConfiguration)
		{
			if(!Enum.IsDefined(typeof(RecordConfigurationNumberStylesChooser), recordConfigurationNumberStylesChooser))
				throw new NotSupportedException($"Unsupported record configuration number styles chooser: {recordConfigurationNumberStylesChooser}");
			_recordConfigurationNumberStylesChooser = recordConfigurationNumberStylesChooser;
		}

		/// <summary>
		/// Sets number styles used during parsing cell value to a number
		/// </summary>
		/// <param name="numberStyles">Desired number styles for parsing cell value to a number</param>
		/// <returns>This configuration object for methods chaining</returns>
		public TDeserializerConfigurator SetNumberStyles(NumberStyles numberStyles)
		{
			_numberStyles = numberStyles;
			return (TDeserializerConfigurator)this;
		}

		/// <summary>
		/// Sets format provider used during parsing cell value to <typeparamref name="TDeserialized"/> type. If null, <see cref="WojciechMikołajewicz.CsvReader.RecordConfiguration.DefaultCulture"/> is used.
		/// </summary>
		/// <param name="formatProvider">Desired format provider for parsing cell value to <typeparamref name="TDeserialized"/> type</param>
		/// <returns>This configuration object for methods chaining</returns>
		public TDeserializerConfigurator SetFormatProvider(IFormatProvider? formatProvider)
		{
			_formatProvider = formatProvider;
			return (TDeserializerConfigurator)this;
		}
	}
}