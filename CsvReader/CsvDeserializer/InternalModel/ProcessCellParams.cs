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
			Record = record;
			ColumnIndex = 0;
			BinderIndex = 0;
		}
	}
}