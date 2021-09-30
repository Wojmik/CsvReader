using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader.CsvNodes
{
	public readonly struct MemorySequenceNode
	{
		public readonly MemorySequenceSpan MemorySequence;

		public readonly NodeType NodeType;

		public MemorySequenceNode(in MemorySequencePosition<char> startPosition, in MemorySequencePosition<char> endPosition, IReadOnlyList<MemorySequencePosition<char>> skipCharPositions, NodeType nodeType)
			: this(new MemorySequenceSpan(startPosition, endPosition, skipCharPositions), nodeType)
		{ }

		public MemorySequenceNode(in MemorySequenceSpan memorySequence, NodeType nodeType)
		{
			this.MemorySequence = memorySequence;
			this.NodeType = nodeType;
		}
	}
}