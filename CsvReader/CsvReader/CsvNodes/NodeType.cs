using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvNodes
{
	/// <summary>
	/// Node type
	/// </summary>
	public enum NodeType
	{
		/// <summary>
		/// Cell
		/// </summary>
		Cell,

		/// <summary>
		/// New line
		/// </summary>
		NewLine,

		/// <summary>
		/// End of stream
		/// </summary>
		EndOfStream,
	}
}