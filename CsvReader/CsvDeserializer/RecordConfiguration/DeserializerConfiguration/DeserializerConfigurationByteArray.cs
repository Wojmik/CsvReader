using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationByteArray<TRecord> : DeserializerConfigurationBase<TRecord, byte[]?>
	{
		private bool? _EmptyAsNull;
		public bool EmptyAsNull { get => _EmptyAsNull??RecordConfiguration.EmptyAsNull; }

		private ByteArrayEncoding? _ByteArrayEncoding;
		public ByteArrayEncoding ByteArrayEncoding { get => _ByteArrayEncoding??RecordConfiguration.ByteArrayEncoding; }

		public DeserializerConfigurationByteArray(PropertyConfigurationBase<TRecord, byte[]?> propertyConfiguration)
			: base(propertyConfiguration)
		{ }

		public DeserializerConfigurationByteArray<TRecord> SetEmptyBehavior(bool emptyAsNull)
		{
			_EmptyAsNull = emptyAsNull;
			return this;
		}

		public DeserializerConfigurationByteArray<TRecord> SetByteArrayEncoding(ByteArrayEncoding byteArrayEncoding)
		{
			_ByteArrayEncoding = byteArrayEncoding;
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
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