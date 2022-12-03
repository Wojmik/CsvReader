using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.Exceptions
{
	/// <summary>
	/// Deserialization exception
	/// </summary>
	public class DeserializationException : SerializationException
	{
		/// <summary>
		/// Zero based row index
		/// </summary>
		public int RowIndex { get; }

		/// <summary>
		/// Zero based column index
		/// </summary>
		public int ColumnIndex { get; }

		/// <summary>
		/// Column name
		/// </summary>
		public string? ColumnName { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rowIndex">Zero based row index</param>
		/// <param name="columnIndex">Zero based column index</param>
		/// <param name="columnName">Column name</param>
		/// <param name="message">Error Message</param>
		public DeserializationException(int rowIndex, int columnIndex, string? columnName, string message)
			: base(message)
		{
			RowIndex = rowIndex;
			ColumnIndex = columnIndex;
			ColumnName = columnName;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rowIndex">Zero based row index</param>
		/// <param name="columnIndex">Zero based column index</param>
		/// <param name="columnName">Column name</param>
		/// <param name="message">Error Message</param>
		/// <param name="innerException">Inner exception</param>
		public DeserializationException(int rowIndex, int columnIndex, string? columnName, string message, Exception innerException)
			: base(message, innerException)
		{
			RowIndex = rowIndex;
			ColumnIndex = columnIndex;
			ColumnName = columnName;
		}
	}
}