using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader
{
	public readonly struct CharArrayNode
	{
		public readonly char[] Data;

		public readonly int Start;

		public readonly int Count;

		public readonly NodeType NodeType;

		public readonly bool Escaped;

		public CharArrayNode(char[] data, int start, int count, NodeType nodeType, bool escaped)
		{
			this.Data=data;
			this.Start=start;
			this.Count=count;
			this.NodeType=nodeType;
			this.Escaped=escaped;
		}
	}
}