using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvNodes
{
	public readonly struct MemoryNode
	{
		public readonly ReadOnlyMemory<char> Data;

		public readonly NodeType NodeType;

		public MemoryNode(in ReadOnlyMemory<char> data, NodeType nodeType)
		{
			this.Data=data;
			this.NodeType=nodeType;
		}
	}
}