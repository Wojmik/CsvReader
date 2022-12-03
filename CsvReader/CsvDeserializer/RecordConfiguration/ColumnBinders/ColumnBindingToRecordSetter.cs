using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordSetter;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders
{
	sealed class ColumnBindingToRecordSetter<TRecord, TProperty> : ColumnBinding<TRecord>
	{
		private readonly CellDeserializerBase<TProperty> _cellDeserializer;

		private readonly RecordSetterBase<TRecord, TProperty> _recordSetter;

		public override NodeContainerType InputType { get => _cellDeserializer.InputType; }

		public ColumnBindingToRecordSetter(string? columnName, int columnIndex, bool optional, CellDeserializerBase<TProperty> cellDeserializer, RecordSetterBase<TRecord, TProperty> recordSetter)
			: base(columnName, columnIndex, optional)
		{
			_cellDeserializer = cellDeserializer;
			_recordSetter = recordSetter;
		}

		public override void Deserialize(TRecord record, in NodeContainer nodeContainer)
		{
			var deserialized = _cellDeserializer.DeserializeCell(nodeContainer);
			_recordSetter.SetRecordData(record, deserialized);
		}
	}
}