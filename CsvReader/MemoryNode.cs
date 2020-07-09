using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader
{
	public readonly struct MemoryNode
	{
		public readonly ReadOnlyMemory<char> Data;

		public readonly NodeType NodeType;

		public readonly bool Escaped;

		public MemoryNode(in ReadOnlyMemory<char> data, NodeType nodeType, bool escaped)
		{
			this.Data=data;
			this.NodeType=nodeType;
			this.Escaped=escaped;
		}
	}
}