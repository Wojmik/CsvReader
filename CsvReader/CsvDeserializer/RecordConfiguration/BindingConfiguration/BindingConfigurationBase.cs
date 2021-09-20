using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration
{
	public abstract class BindingConfigurationBase<TRecord>
	{
		protected internal RecordConfiguration<TRecord> RecordConfiguration { get; }

		public string? ColumnName { get; protected set; }

		public int ColumnIndex { get; protected set; }

		protected BindingConfigurationBase(RecordConfiguration<TRecord> recordConfiguration)
		{
			this.RecordConfiguration = recordConfiguration;
			this.ColumnIndex = -1;
		}

		internal abstract bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out ColumnBinding<TRecord>? columnBinding);
	}
}