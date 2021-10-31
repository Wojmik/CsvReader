using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Deserializer configurator for <see cref="byte"/> <see cref="Array"/> type
	/// </summary>
	public class DeserializerConfigurationByteArray : DeserializerConfigurationBase<byte[]?>
	{
		private bool? _EmptyAsNull;
		/// <summary>
		/// Treat empty csv cell as null
		/// </summary>
		public bool EmptyAsNull { get => _EmptyAsNull??RecordConfiguration.DefaultEmptyAsNull; }

		private ByteArrayEncoding? _ByteArrayEncoding;
		/// <summary>
		/// Byte array encoding
		/// </summary>
		public ByteArrayEncoding ByteArrayEncoding { get => _ByteArrayEncoding??RecordConfiguration.DefaultByteArrayEncoding; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationByteArray(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{ }

		/// <summary>
		/// Sets empty csv cell behavior
		/// </summary>
		/// <param name="emptyAsNull">True to return null for empty csv cell, false to return empty array</param>
		/// <returns>This configuration object for methods chaining</returns>
		public DeserializerConfigurationByteArray SetEmptyBehavior(bool emptyAsNull)
		{
			_EmptyAsNull = emptyAsNull;
			return this;
		}

		/// <summary>
		/// Sets <see cref="byte"/> <see cref="Array"/> encoding
		/// </summary>
		/// <param name="byteArrayEncoding">Byte array encoding to set</param>
		/// <returns>This configuration object for methods chaining</returns>
		public DeserializerConfigurationByteArray SetByteArrayEncoding(ByteArrayEncoding byteArrayEncoding)
		{
			_ByteArrayEncoding = byteArrayEncoding;
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<byte[]?>? cellDeserializer)
		{
			switch(ByteArrayEncoding)
			{
				case ByteArrayEncoding.Base64:
					cellDeserializer = new CellByteArrayBase64Deserializer(EmptyAsNull);
					break;
				case ByteArrayEncoding.Hex:
					cellDeserializer = new CellByteArrayHexDeserializer(EmptyAsNull);
					break;
				default:
					throw new NotSupportedException($"Byte array encoding not supported: {ByteArrayEncoding}");
			}
			return true;
		}
	}
}