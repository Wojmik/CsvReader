using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvNodes;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers
{
	/// <summary>
	/// Node container type
	/// </summary>
	public enum NodeContainerType
	{
		/// <summary>
		/// Empty
		/// </summary>
		Empty,

		/// <summary>
		/// Csv cell read as <see cref="MemorySequenceSpan"/>
		/// </summary>
		MemorySequence,

		/// <summary>
		/// Csv cell read as <see cref="ReadOnlyMemory{T}"/>
		/// </summary>
		Memory,

		/// <summary>
		/// Csv cell read as <see cref="string"/>
		/// </summary>
		String,
	}
}