namespace WojciechMikołajewicz.CsvReader
{
	/// <summary>
	/// <see cref="CsvReader"/> options
	/// </summary>
	public interface ICsvReaderOptions
	{
		/// <summary>
		/// Data can be enclosed with escape characters to store control characters. Default is true
		/// </summary>
		bool CanEscape { get; set; }

		/// <summary>
		/// Permits empty line at the end of the file. If false, empty line at the end of file is interpreted as record. Default is true
		/// </summary>
		bool PermitEmptyLineAtEnd { get; set; }

		/// <summary>
		/// Escape character. Default is '"'
		/// </summary>
		char EscapeChar { get; set; }

		/// <summary>
		/// Cells delimiter character. Default is ','
		/// </summary>
		char DelimiterChar { get; set; }

		/// <summary>
		/// Line ending. Default is <see cref="LineEnding.Auto"/>
		/// </summary>
		LineEnding LineEnding { get; set; }

		/// <summary>
		/// Minimum size of single buffer segment in chars. Default is 32768
		/// </summary>
		int BufferSizeInChars { get; set; }

		/// <summary>
		/// Leave underlying <see cref="System.IO.TextReader"/> open. Default is false
		/// </summary>
		bool LeaveOpen { get; set; }
	}
}