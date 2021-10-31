using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	/// <summary>
	/// Base class for cell deserializers from <see cref="ReadOnlyMemory{T}"/> read from csv
	/// </summary>
	/// <typeparam name="TDeserialized">Type of data to deserialize cell to</typeparam>
	public abstract class CellDeserializerFromMemoryBase<TDeserialized> : CellDeserializerBase<TDeserialized>
	{
		internal sealed override NodeContainerType InputType { get => NodeContainerType.Memory; }

		/// <summary>
		/// Throws <see cref="NotSupportedException"/>. This override is dead end. This method would not be called.
		/// </summary>
		/// <param name="value">Cell read from csv in <see cref="MemorySequenceSpan"/> manner</param>
		/// <returns>Always throws <see cref="NotSupportedException"/></returns>
		/// <exception cref="NotSupportedException">Always throws</exception>
		protected sealed override TDeserialized DeserializeFromMemorySequence(in MemorySequenceSpan value)
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