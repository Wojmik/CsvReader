using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader
{
	public readonly struct MemorySequenceNode
	{
		public readonly MemorySequencePosition<char> StartPosition;
		
		public readonly MemorySequencePosition<char> EndPosition;

		public readonly NodeType NodeType;

		public readonly IReadOnlyList<MemorySequencePosition<char>> SkipCharPositions;

		public MemorySequenceNode(in MemorySequencePosition<char> startPosition, in MemorySequencePosition<char> endPosition, NodeType nodeType, IReadOnlyList<MemorySequencePosition<char>> skipCharPositions)
		{
			this.StartPosition=startPosition;
			this.EndPosition=endPosition;
			this.NodeType=nodeType;
			this.SkipCharPositions=skipCharPositions;
		}
	}
}