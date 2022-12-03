using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordSetter
{
	sealed class RecordSetter<TRecord, TProperty> : RecordSetterBase<TRecord, TProperty>
	{
		private readonly Action<TRecord, TProperty> _recordSetterMethod;

		public RecordSetter(Action<TRecord, TProperty> recordSetterMethod)
		{
			_recordSetterMethod = recordSetterMethod;
		}

		public override void SetRecordData(TRecord record, TProperty property)
		{
			_recordSetterMethod.Invoke(record, property);
		}
	}
}