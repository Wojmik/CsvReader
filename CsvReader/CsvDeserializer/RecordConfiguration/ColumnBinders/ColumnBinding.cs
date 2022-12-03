using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders
{
	abstract class ColumnBinding<TRecord>
	{
		public string? ColumnName { get; }

		public int ColumnIndex { get; internal set; }

		public abstract NodeContainerType InputType { get; }

		public bool Required { get; }

		public BindingType Type { get => ColumnName!=null ? BindingType.ByColumnName : BindingType.ByColumnIndex; }

		protected ColumnBinding(string? columnName, int columnIndex, bool optional)
		{
			if (columnName == null && columnIndex < 0)
				throw new InvalidOperationException($"{nameof(columnName)} cannot be null and {nameof(columnIndex)} cannot be less than zero simultaneously");

			ColumnName = columnName;
			ColumnIndex = columnIndex;
			Required = !optional;
		}

		public abstract void Deserialize(TRecord record, in NodeContainer nodeContainer);
	}
}