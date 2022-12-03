using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	sealed class CellDeserializerFromString<TDeserialized> : CellDeserializerFromStringBase<TDeserialized>
	{
		private readonly Func<string, TDeserialized> _deserializeMethod;

		public CellDeserializerFromString(Func<string, TDeserialized> deserializeMethod)
		{
			_deserializeMethod = deserializeMethod;
		}

		protected override TDeserialized DeserializeFromString(string value)
		{
			return _deserializeMethod.Invoke(value);
		}
	}
}