using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders
{
	/// <summary>
	/// Binding type
	/// </summary>
	enum BindingType
	{
		/// <summary>
		/// Binding by column index
		/// </summary>
		ByColumnIndex,

		/// <summary>
		/// Binding by column name
		/// </summary>
		ByColumnName,
	}
}