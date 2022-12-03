using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	readonly struct NodeContainer
	{
		private readonly MemorySequenceSpan _memorySequence;
		public MemorySequenceSpan MemorySequence
		{
			get => NodeContainerType==NodeContainerType.MemorySequence ? _memorySequence : throw new InvalidOperationException($"Cannot return {nameof(MemorySequence)}. Current state is {NodeContainerType}");
		}

		private readonly ReadOnlyMemory<char> _memory;
		public ReadOnlyMemory<char> Memory
		{
			get => NodeContainerType==NodeContainerType.Memory ? _memory : throw new InvalidOperationException($"Cannot return {nameof(Memory)}. Current state is {NodeContainerType}");
		}

		private readonly string? _string;
		public string String
		{
			get => NodeContainerType==NodeContainerType.String ? _string! : throw new InvalidOperationException($"Cannot return {nameof(String)}. Current state is {NodeContainerType}");
		}

		public NodeContainerType NodeContainerType { get; }

		public NodeContainer(in MemorySequenceSpan memorySequence)
		{
			_memorySequence = memorySequence;
			_memory = default;
			_string = null;
			NodeContainerType = NodeContainerType.MemorySequence;
		}

		public NodeContainer(in ReadOnlyMemory<char> memory)
		{
			_memorySequence = default;
			_memory = memory;
			_string = null;
			NodeContainerType = NodeContainerType.Memory;
		}

		public NodeContainer(string str)
		{
			_memorySequence = default;
			_memory = default;
			_string = str;
			NodeContainerType = NodeContainerType.String;
		}
	}
}