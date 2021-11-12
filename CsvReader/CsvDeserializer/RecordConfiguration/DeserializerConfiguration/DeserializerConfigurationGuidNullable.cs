﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration
{
	/// <summary>
	/// Deserializer configurator for nullable <see cref="Guid"/> type
	/// </summary>
	public class DeserializerConfigurationGuidNullable : DeserializerConfigurationBase<Guid?>
	{
		/// <summary>
		/// Format used during parsing cell value to target type. If null, standard formats are used.
		/// </summary>
		public string? Format { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bindingConfiguration">Binding to column configuration object</param>
		public DeserializerConfigurationGuidNullable(BindingConfigurationBase bindingConfiguration)
			: base(bindingConfiguration)
		{ }

		/// <summary>
		/// Sets format used during parsing cell value to <see cref="Guid"/> type. If null, standard formats are used.
		/// </summary>
		/// <param name="format">Desired format for parsing cell value to <see cref="Guid"/> type</param>
		/// <returns>This configuration object for methods chaining</returns>
		public DeserializerConfigurationGuidNullable SetFormat(string? format)
		{
			Format = format;
			return this;
		}

		internal override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out CellDeserializerBase<Guid?>? cellDeserializer)
		{
			if(Format!=null)
				cellDeserializer = new CellGuidFormattedNullableDeserializer(Format);
			else
				cellDeserializer = new CellGuidNullableDeserializer();
			return true;
		}
	}
}