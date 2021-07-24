using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader
{
	/// <summary>
	/// Line ending
	/// </summary>
	public enum LineEnding
	{
		/// <summary>
		/// Automatic discovery based on first line
		/// </summary>
		Auto,

		/// <summary>
		/// Line ending used on Windows (Carriage Return + Line Feed)
		/// </summary>
		CRLF,

		/// <summary>
		/// Line ending used on Linux (Line Feed)
		/// </summary>
		LF,

		/// <summary>
		/// Line ending used on old Macintosh (Carriage Return)
		/// </summary>
		CR,
	}
}