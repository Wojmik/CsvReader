using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	/// <summary>
	/// Base class for cell deserializers
	/// </summary>
	/// <typeparam name="TDeserialized">Type of data to deserialize cell to</typeparam>
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

		/// <summary>
		/// Deserialize cell from <see cref="MemorySequenceSpan"/>
		/// </summary>
		/// <param name="value">Cell read from csv in <see cref="MemorySequenceSpan"/> manner</param>
		/// <returns>Deserialized value</returns>
		protected abstract TDeserialized DeserializeFromMemorySequence(in MemorySequenceSpan value);

		/// <summary>
		/// Deserialize cell from <see cref="ReadOnlyMemory{T}"/>
		/// </summary>
		/// <param name="value">Cell read from csv in <see cref="ReadOnlyMemory{T}"/> manner</param>
		/// <returns>Deserialized value</returns>
		protected abstract TDeserialized DeserializeFromMemory(in ReadOnlyMemory<char> value);

		/// <summary>
		/// Deserialize cell from <see cref="string"/>
		/// </summary>
		/// <param name="value">Cell read from csv in <see cref="string"/> manner</param>
		/// <returns>Deserialized value</returns>
		protected abstract TDeserialized DeserializeFromString(string value);
	}
}