using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvNodes
{
	/// <summary>
	/// Csv node with data as <see cref="string"/>
	/// </summary>
	public readonly struct StringNode
	{
		/// <summary>
		/// Cell value
		/// </summary>
		public readonly string Data;

		/// <summary>
		/// Node type
		/// </summary>
		public readonly NodeType NodeType;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Cell value</param>
		/// <param name="nodeType">Node type</param>
		public StringNode(string data, NodeType nodeType)
		{
			this.Data=data;
			this.NodeType=nodeType;
		}
	}
}