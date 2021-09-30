using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	public abstract class CellDeserializerFromMemorySequenceBase<TDeserialized> : CellDeserializerBase<TDeserialized>
	{
		internal sealed override NodeContainerType InputType { get => NodeContainerType.MemorySequence; }

		protected sealed override TDeserialized DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			throw new NotSupportedException();
		}

		protected sealed override TDeserialized DeserializeFromString(string value)
		{
			throw new NotSupportedException();
		}
	}
}