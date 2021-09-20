using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordSetter
{
	sealed class RecordSetter<TRecord, TProperty> : RecordSetterBase<TRecord, TProperty>
	{
		private readonly Action<TRecord, TProperty> RecordSetterMethod;

		public RecordSetter(Action<TRecord, TProperty> recordSetterMethod)
		{
			this.RecordSetterMethod = recordSetterMethod;
		}

		public override void SetRecordData(TRecord record, TProperty property)
		{
			RecordSetterMethod.Invoke(record, property);
		}
	}
}