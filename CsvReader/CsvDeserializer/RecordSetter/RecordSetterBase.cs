using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordSetter
{
	abstract class RecordSetterBase<TRecord, TProperty>
	{
		public abstract void SetRecordData(TRecord record, TProperty property);
	}
}