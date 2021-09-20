using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.InternalModel
{
	struct ProcessCellParams<TRecord>
	{
		public readonly TRecord Record;

		public int ColumnIndex;

		public int BinderIndex;

		public ProcessCellParams(TRecord record)
		{
			this.Record = record;
			this.ColumnIndex = 0;
			this.BinderIndex = 0;
		}
	}
}