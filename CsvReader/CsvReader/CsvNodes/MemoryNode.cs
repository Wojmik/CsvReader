using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvNodes
{
	/// <summary>
	/// Csv node with data as <see cref="ReadOnlyMemory{T}"/>
	/// </summary>
	public readonly struct MemoryNode
	{
		/// <summary>
		/// Cell value
		/// </summary>
		public readonly ReadOnlyMemory<char> Data;

		/// <summary>
		/// Node type
		/// </summary>
		public readonly NodeType NodeType;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Cell value</param>
		/// <param name="nodeType">Node type</param>
		public MemoryNode(in ReadOnlyMemory<char> data, NodeType nodeType)
		{
			this.Data=data;
			this.NodeType=nodeType;
		}
	}
}