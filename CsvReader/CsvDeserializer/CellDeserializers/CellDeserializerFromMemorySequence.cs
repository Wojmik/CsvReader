using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvNodes;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellDeserializerFromMemorySequence<TDeserialized> : CellDeserializerFromMemorySequenceBase<TDeserialized>
	{
		private readonly Func<MemorySequenceSpan, TDeserialized> DeserializeMethod;

		public CellDeserializerFromMemorySequence(Func<MemorySequenceSpan, TDeserialized> deserializeMethod)
		{
			this.DeserializeMethod = deserializeMethod;
		}

		protected override TDeserialized DeserializeFromMemorySequence(in MemorySequenceSpan value)
		{
			return DeserializeMethod.Invoke(value);
		}
	}
}