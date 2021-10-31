using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// <see cref="byte"/> <see cref="Array"/> encoding type
	/// </summary>
	public enum ByteArrayEncoding
	{
		/// <summary>
		/// Base64 encoding
		/// </summary>
		Base64,

		/// <summary>
		/// Hex encoding
		/// </summary>
		Hex,
	}
}