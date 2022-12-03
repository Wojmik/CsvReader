using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders
{
	sealed class ColumnBindingToProperty<TRecord, TProperty> : ColumnBinding<TRecord>
	{
		private readonly CellDeserializerBase<TProperty> _cellDeserializer;

		private readonly Action<TRecord, TProperty> _setPropertyMethod;

		public override NodeContainerType InputType { get => _cellDeserializer.InputType; }

		public ColumnBindingToProperty(string? columnName, int columnIndex, bool optional, CellDeserializerBase<TProperty> cellDeserializer, Action<TRecord, TProperty> setPropertyMethod)
			: base(columnName, columnIndex, optional)
		{
			_cellDeserializer = cellDeserializer;
			_setPropertyMethod = setPropertyMethod;
		}

		public override void Deserialize(TRecord record, in NodeContainer nodeContainer)
		{
			var deserialized = _cellDeserializer.DeserializeCell(nodeContainer);
			_setPropertyMethod.Invoke(record, deserialized);
		}
	}
}