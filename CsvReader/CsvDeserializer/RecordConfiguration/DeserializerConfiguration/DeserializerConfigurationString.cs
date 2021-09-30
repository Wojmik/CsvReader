using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	public class DeserializerConfigurationString<TRecord> : DeserializerConfigurationBase<TRecord, string?>
	{
		private bool? _EmptyAsNull;
		public bool EmptyAsNull { get => _EmptyAsNull??RecordConfiguration.EmptyAsNull; }

		private bool? _DeduplicateStrings;
		public bool DeduplicateStrings { get => _DeduplicateStrings??RecordConfiguration.DeduplicateStrings; }

		public DeserializerConfigurationString(PropertyConfigurationBase<TRecord, string?> propertyConfiguration)
			: base(propertyConfiguration)
		{ }

		public DeserializerConfigurationString<TRecord> SetEmptyStringBehavior(bool emptyAsNull)
		{
			_EmptyAsNull = emptyAsNull;
			return this;
		}

		public DeserializerConfigurationString<TRecord> SetStringsDeduplicatingBehavior(bool deduplicateStrings)
		{
			_DeduplicateStrings = deduplicateStrings;
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
# endif
			out CellDeserializerBase<string?>? cellDeserializer)
		{
			if(DeduplicateStrings)
				cellDeserializer = new CellStringDeduplicatedDeserializer(EmptyAsNull, RecordConfiguration.StringDeduplicator);
			else
				cellDeserializer = new CellStringDeserializer(EmptyAsNull);
			return true;
		}
	}
}