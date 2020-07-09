using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader
{
	public enum NodeType
	{
		EndOfStream,

		Cell,

		NewLine,
	}
}