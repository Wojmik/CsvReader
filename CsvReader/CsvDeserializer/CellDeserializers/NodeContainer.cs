using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	readonly struct NodeContainer
	{
		private readonly MemorySequenceSpan _MemorySequence;
		public MemorySequenceSpan MemorySequence
		{
			get => NodeContainerType==NodeContainerType.MemorySequence ? _MemorySequence : throw new InvalidOperationException($"Cannot return {nameof(MemorySequence)}. Current state is {NodeContainerType}");
		}

		private readonly ReadOnlyMemory<char> _Memory;
		public ReadOnlyMemory<char> Memory
		{
			get => NodeContainerType==NodeContainerType.Memory ? _Memory : throw new InvalidOperationException($"Cannot return {nameof(Memory)}. Current state is {NodeContainerType}");
		}

		private readonly string? _String;
		public string String
		{
			get => NodeContainerType==NodeContainerType.String ? _String! : throw new InvalidOperationException($"Cannot return {nameof(String)}. Current state is {NodeContainerType}");
		}

		public NodeContainerType NodeContainerType { get; }

		public NodeContainer(in MemorySequenceSpan memorySequence)
		{
			this._MemorySequence = memorySequence;
			this._Memory = default;
			this._String = null;
			this.NodeContainerType = NodeContainerType.MemorySequence;
		}

		public NodeContainer(in ReadOnlyMemory<char> memory)
		{
			this._MemorySequence = default;
			this._Memory = memory;
			this._String = null;
			this.NodeContainerType = NodeContainerType.Memory;
		}

		public NodeContainer(string str)
		{
			this._MemorySequence = default;
			this._Memory = default;
			this._String = str;
			this.NodeContainerType = NodeContainerType.String;
		}
	}
}