using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordSetter;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders
{
	abstract class ColumnBinding<TRecord>
	{
		public string? ColumnName { get; }

		public int ColumnIndex { get; internal set; }

		public abstract NodeContainerType InputType { get; }

		protected ColumnBinding(string? columnName, int columnIndex)
		{
			this.ColumnName = columnName;
			this.ColumnIndex = columnIndex;
		}

		public abstract void Deserialize(TRecord record, in NodeContainer nodeContainer);
	}
}