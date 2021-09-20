using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordSetter;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders
{
	sealed class ColumnBindingToRecordSetter<TRecord, TProperty> : ColumnBinding<TRecord>
	{
		private readonly CellDeserializerBase<TProperty> CellDeserializer;

		private readonly RecordSetterBase<TRecord, TProperty> RecordSetter;

		public override NodeContainerType InputType { get => CellDeserializer.InputType; }

		public ColumnBindingToRecordSetter(string? columnName, int columnIndex, CellDeserializerBase<TProperty> cellDeserializer, RecordSetterBase<TRecord, TProperty> recordSetter)
			: base(columnName, columnIndex)
		{
			this.CellDeserializer = cellDeserializer;
			this.RecordSetter = recordSetter;
		}

		public override void Deserialize(TRecord record, in NodeContainer nodeContainer)
		{
			var deserialized = CellDeserializer.DeserializeCell(nodeContainer);
			RecordSetter.SetRecordData(record, deserialized);
		}
	}
}