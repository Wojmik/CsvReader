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

		public string? ColumnName { get; private protected set; }

		public int ColumnIndex { get; private protected set; }

		protected BindingConfigurationBase(RecordConfiguration<TRecord> recordConfiguration)
		{
			this.RecordConfiguration = recordConfiguration;
			this.ColumnIndex = -1;
		}

		protected internal void BindToColumnInternal(string columnName)
		{
			ColumnName = columnName??throw new ArgumentNullException(nameof(columnName));
			ColumnIndex = -1;
		}

		protected internal void BindToColumnInternal(int columnIndex)
		{
			if(columnIndex<0)
				throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, $"{nameof(columnIndex)} cannot be less than zero");
			ColumnName = null;
			ColumnIndex = columnIndex;
		}

		internal abstract bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			out ColumnBinding<TRecord>? columnBinding);
	}
}