using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationBool<TRecord> : DeserializerConfigurationNotNullableBase<TRecord, bool, DeserializerConfigurationBool<TRecord>>
	{
		public string TrueString { get; private set; }

		public string FalseString { get; private set; }

		public DeserializerConfigurationBool(PropertyConfigurationBase<TRecord, bool> propertyConfiguration)
			: base(propertyConfiguration)
		{
			TrueString = bool.TrueString;
			FalseString = bool.FalseString;
		}

		public DeserializerConfigurationBool<TRecord> SetTrueString(string trueString)
		{
			TrueString = trueString??throw new ArgumentNullException(nameof(trueString));
			return this;
		}

		public DeserializerConfigurationBool<TRecord> SetFalseString(string falseString)
		{
			FalseString = falseString??throw new ArgumentNullException(nameof(falseString));
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<bool>? cellDeserializer)
		{
			cellDeserializer = new CellBoolDeserializer(TrueString, FalseString, AllowEmpty, ValueForEmpty);
			return true;
		}
	}
}