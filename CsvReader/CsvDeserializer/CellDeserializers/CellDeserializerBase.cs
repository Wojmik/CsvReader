using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	public abstract class CellDeserializerBase<TDeserialized>
	{
		internal abstract NodeContainerType InputType { get; }

		internal CellDeserializerBase()
		{ }

		internal TDeserialized DeserializeCell(in NodeContainer nodeContainer)
		{
			TDeserialized deserialized;

			switch(InputType)
			{
				case NodeContainerType.MemorySequence:
					deserialized = DeserializeFromMemorySequence(nodeContainer.MemorySequence);
					break;
				case NodeContainerType.Memory:
					deserialized = DeserializeFromMemory(nodeContainer.Memory);
					break;
				case NodeContainerType.String:
					deserialized = DeserializeFromString(nodeContainer.String);
					break;
				default:
					throw new NotSupportedException($"Unsupported input type: {InputType}");
			}

			return deserialized;
		}

		protected abstract TDeserialized DeserializeFromMemorySequence(in MemorySequenceSpan value);

		protected abstract TDeserialized DeserializeFromMemory(in ReadOnlyMemory<char> value);

		protected abstract TDeserialized DeserializeFromString(string value);
	}
}