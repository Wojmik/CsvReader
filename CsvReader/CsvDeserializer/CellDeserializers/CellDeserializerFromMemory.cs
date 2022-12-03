using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellDeserializerFromMemory<TDeserialized> : CellDeserializerFromMemoryBase<TDeserialized>
	{
		private readonly Func<ReadOnlyMemory<char>, TDeserialized> _deserializeMethod;

		public CellDeserializerFromMemory(Func<ReadOnlyMemory<char>, TDeserialized> deserializeMethod)
		{
			_deserializeMethod = deserializeMethod;
		}

		protected override TDeserialized DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			return _deserializeMethod.Invoke(value);
		}
	}
}