using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	/// <summary>
	/// Base class for cell deserializers from <see cref="MemorySequenceSpan"/> read from csv
	/// </summary>
	/// <typeparam name="TDeserialized">Type of data to deserialize cell to</typeparam>
	public abstract class CellDeserializerFromMemorySequenceBase<TDeserialized> : CellDeserializerBase<TDeserialized>
	{
		internal sealed override NodeContainerType InputType { get => NodeContainerType.MemorySequence; }

		/// <summary>
		/// Throws <see cref="NotSupportedException"/>. This override is dead end. This method would not be called.
		/// </summary>
		/// <param name="value">Cell read from csv in <see cref="ReadOnlyMemory{T}"/> manner</param>
		/// <returns>Always throws <see cref="NotSupportedException"/></returns>
		/// <exception cref="NotSupportedException">Always throws</exception>
		protected sealed override TDeserialized DeserializeFromMemory(in ReadOnlyMemory<char> value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Throws <see cref="NotSupportedException"/>. This override is dead end. This method would not be called.
		/// </summary>
		/// <param name="value">Cell read from csv in <see cref="string"/> manner</param>
		/// <returns>Always throws <see cref="NotSupportedException"/></returns>
		/// <exception cref="NotSupportedException">Always throws</exception>
		protected sealed override TDeserialized DeserializeFromString(string value)
		{
			throw new NotSupportedException();
		}
	}
}