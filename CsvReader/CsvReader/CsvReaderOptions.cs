using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader
{
	/// <summary>
	/// <see cref="CsvReader"/> options
	/// </summary>
	class CsvReaderOptions : ICsvReaderOptions
	{
		const bool CanEscapeDefault = true;
		const bool PermitEmptyLineAtEndDefault = true;
		const char EscapeCharDefault = '\"';
		const char DelimiterCharDefault = ',';
		const LineEnding LineEndingDefault = LineEnding.Auto;
		const int BufferSizeInCharsDefault = 32 * 1024;

		/// <summary>
		/// Data can be enclosed with escape characters to store control characters. Default is true
		/// </summary>
		public bool CanEscape { get; set; }

		/// <summary>
		/// Permits empty line at the end of the file. If false, empty line at the end of file is interpreted as record. Default is true
		/// </summary>
		public bool PermitEmptyLineAtEnd { get; set; }

		/// <summary>
		/// Escape character. Default is '"'
		/// </summary>
		public char EscapeChar { get; set; }

		/// <summary>
		/// Cells delimiter character. Default is ','
		/// </summary>
		public char DelimiterChar { get; set; }

		/// <summary>
		/// Line ending. Default is <see cref="LineEnding.Auto"/>
		/// </summary>
		public LineEnding LineEnding { get; set; }

		/// <summary>
		/// Minimum size of single buffer segment in chars. Default is 32768
		/// </summary>
		public int BufferSizeInChars { get; set; }

		/// <summary>
		/// Leave underlying <see cref="System.IO.TextReader"/> open. Default is false
		/// </summary>
		public bool LeaveOpen { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public CsvReaderOptions()
		{
			CanEscape = CanEscapeDefault;
			PermitEmptyLineAtEnd = PermitEmptyLineAtEndDefault;
			EscapeChar = EscapeCharDefault;
			DelimiterChar = DelimiterCharDefault;
			LineEnding = LineEndingDefault;
			BufferSizeInChars = BufferSizeInCharsDefault;
		}
	}
}