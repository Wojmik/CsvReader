using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Choosing property of <see cref="WojciechMikołajewicz.CsvReader.RecordConfiguration"/> for default <see cref="NumberStyles"/>
	/// </summary>
	public enum RecordConfigurationNumberStylesChooser
	{
		/// <summary>
		/// Default integer number styles property
		/// </summary>
		IntegerNumberStyles,

		/// <summary>
		/// Default floating point number styles property
		/// </summary>
		FloatingPointNumberStyles,

		/// <summary>
		/// Default <see cref="decimal"/> number styles property
		/// </summary>
		DecimalNumberStyles,
	}
}