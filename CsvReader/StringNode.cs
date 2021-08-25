using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader
{
	public readonly struct StringNode
	{
		public readonly string Data;

		public readonly NodeType NodeType;

		public StringNode(string data, NodeType nodeType)
		{
			this.Data=data;
			this.NodeType=nodeType;
		}
	}
}