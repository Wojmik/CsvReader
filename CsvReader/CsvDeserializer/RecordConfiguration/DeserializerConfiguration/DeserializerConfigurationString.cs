using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Deserializer configurator for <see cref="string"/> type
	/// </summary>
	public class DeserializerConfigurationString : DeserializerConfigurationBase<string?>
	{
		private bool? _emptyAsNull;
		/// <summary>
		/// Treat empty csv cell as null
		/// </summary>
		public bool EmptyAsNull { get => _emptyAsNull??RecordConfiguration.DefaultEmptyAsNull; }

		private bool? _deduplicateStrings;
		/// <summary>
		/// Deduplicate strings with the same value
		/// </summary>
		public bool DeduplicateStrings { get => _deduplicateStrings??RecordConfiguration.DefaultDeduplicateStrings; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationString(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{ }

		/// <summary>
		/// Sets empty csv cell behavior
		/// </summary>
		/// <param name="emptyAsNull">True to return null for empty csv cell, false to return empty string</param>
		/// <returns>This configuration object for methods chaining</returns>
		public DeserializerConfigurationString SetEmptyStringBehavior(bool emptyAsNull)
		{
			_emptyAsNull = emptyAsNull;
			return this;
		}

		/// <summary>
		/// Sets string deduplication behavior
		/// </summary>
		/// <param name="deduplicateStrings">True to deduplicate strings with the same value, false otherwise</param>
		/// <returns>This configuration object for methods chaining</returns>
		public DeserializerConfigurationString SetStringsDeduplicatingBehavior(bool deduplicateStrings)
		{
			_deduplicateStrings = deduplicateStrings;
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
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