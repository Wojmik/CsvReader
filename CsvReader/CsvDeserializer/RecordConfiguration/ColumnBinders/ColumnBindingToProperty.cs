using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders
{
	sealed class ColumnBindingToProperty<TRecord, TProperty> : ColumnBinding<TRecord>
	{
		private readonly CellDeserializerBase<TProperty> CellDeserializer;

		private readonly Action<TRecord, TProperty> SetPropertyMethod;

		public override NodeContainerType InputType { get => CellDeserializer.InputType; }

		public ColumnBindingToProperty(string? columnName, int columnIndex, CellDeserializerBase<TProperty> cellDeserializer, Action<TRecord, TProperty> setPropertyMethod)
			: base(columnName, columnIndex)
		{
			this.CellDeserializer = cellDeserializer;
			this.SetPropertyMethod = setPropertyMethod;
		}

		public override void Deserialize(TRecord record, in NodeContainer nodeContainer)
		{
			var deserialized = CellDeserializer.DeserializeCell(nodeContainer);
			SetPropertyMethod.Invoke(record, deserialized);
		}
	}
}