using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationBoolNullable<TRecord> : DeserializerConfigurationBase<TRecord, bool?>
	{
		public string TrueString { get; private set; }

		public string FalseString { get; private set; }

		public DeserializerConfigurationBoolNullable(PropertyConfigurationBase<TRecord, bool?> propertyConfiguration)
			: base(propertyConfiguration)
		{
			TrueString = bool.TrueString;
			FalseString = bool.FalseString;
		}

		public DeserializerConfigurationBoolNullable<TRecord> SetTrueString(string trueString)
		{
			TrueString = trueString??throw new ArgumentNullException(nameof(trueString));
			return this;
		}

		public DeserializerConfigurationBoolNullable<TRecord> SetFalseString(string falseString)
		{
			FalseString = falseString??throw new ArgumentNullException(nameof(falseString));
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<bool?>? cellDeserializer)
		{
			cellDeserializer = new CellBoolNullableDeserializer(TrueString, FalseString);
			return true;
		}
	}
}