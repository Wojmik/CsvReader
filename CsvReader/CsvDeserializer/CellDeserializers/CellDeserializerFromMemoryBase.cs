using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	public abstract class CellDeserializerFromMemoryBase<TDeserialized> : CellDeserializerBase<TDeserialized>
	{
		internal sealed override NodeContainerType InputType { get => NodeContainerType.Memory; }

		protected sealed override TDeserialized DeserializeFromMemorySequence(in MemorySequenceSpan value)
		{
			throw new NotSupportedException();
		}

		protected sealed override TDeserialized DeserializeFromString(string value)
		{
			throw new NotSupportedException();
		}
	}
}