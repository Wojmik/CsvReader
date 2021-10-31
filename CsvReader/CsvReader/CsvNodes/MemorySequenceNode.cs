using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader.CsvNodes
{
	/// <summary>
	/// Csv node with data as <see cref="MemorySequenceSpan"/>
	/// </summary>
	public readonly struct MemorySequenceNode
	{
		/// <summary>
		/// Cell value
		/// </summary>
		public readonly MemorySequenceSpan MemorySequence;

		/// <summary>
		/// Node type
		/// </summary>
		public readonly NodeType NodeType;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="startPosition">Start position</param>
		/// <param name="endPosition">End position</param>
		/// <param name="skipCharPositions">Positions of chars to skip</param>
		/// <param name="nodeType">Node type</param>
		public MemorySequenceNode(in MemorySequencePosition<char> startPosition, in MemorySequencePosition<char> endPosition, IReadOnlyList<MemorySequencePosition<char>> skipCharPositions, NodeType nodeType)
			: this(new MemorySequenceSpan(startPosition, endPosition, skipCharPositions), nodeType)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="memorySequence">Cell value</param>
		/// <param name="nodeType">Node type</param>
		public MemorySequenceNode(in MemorySequenceSpan memorySequence, NodeType nodeType)
		{
			this.MemorySequence = memorySequence;
			this.NodeType = nodeType;
		}
	}
}