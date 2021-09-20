using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	public abstract class CellDeserializerFromStringBase<TDeserialized> : CellDeserializerBase<TDeserialized>
	{
		internal sealed override NodeContainerType InputType { get => NodeContainerType.String; }

		protected override sealed TDeserialized DeserializeFromMemorySequence(in MemorySequenceSpan value)
		{
			throw new NotSupportedException();
		}

		protected override sealed TDeserialized DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			throw new NotSupportedException();
		}
	}
}