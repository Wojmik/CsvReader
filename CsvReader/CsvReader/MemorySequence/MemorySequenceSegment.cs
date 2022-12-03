using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReader.MemorySequence
{
	/// <summary>
	/// Segment of continuest memory of <see cref="MemorySequence{T}"/>
	/// </summary>
	/// <typeparam name="T">Type of elements</typeparam>
	class MemorySequenceSegment<T> : ReadOnlySequenceSegment<T>, IDisposable
	{
		/// <summary>
		/// Array for data
		/// </summary>
		internal T[] Array { get; private set; }

		/// <summary>
		/// Number of elements stored in <see cref="Array"/>
		/// </summary>
		internal int Count { get => this.Memory.Length; set => this.Memory = new ReadOnlyMemory<T>(this.Array, 0, value); }

		/// <summary>
		/// Next node of MemorySequenceSegment&lt;T&gt; type
		/// </summary>
		internal MemorySequenceSegment<T>? NextInternal { get; private set; }

		///// <summary>
		///// Reference to next <see cref="MemorySequenceSegment{T}"/>
		///// </summary>
		//internal MemorySequenceSegment<T> InternalNext { get; private set; }

		/// <summary>
		/// Constructor - creates new <see cref="MemorySequenceSegment{T}"/> and rents memory for it
		/// </summary>
		/// <param name="previous">Previous <see cref="MemorySequenceSegment{T}"/>. This <see cref="MemorySequenceSegment{T}"/> is added as <see cref="ReadOnlySequenceSegment{T}.Next"/> of <paramref name="previous"/></param>
		/// <param name="minimumLength">Minimum memory size for this <see cref="MemorySequenceSegment{T}"/></param>
		internal MemorySequenceSegment(MemorySequenceSegment<T>? previous, int minimumLength)
		{
			if (previous != null)
			{
				previous.NextInternal = this;
				previous.Next = this;
				RunningIndex = previous.RunningIndex + previous.Array.Length;
			}

			Array = ArrayPool<T>.Shared.Rent(minimumLength);
		}

		/// <summary>
		/// Reuses this <see cref="MemorySequenceSegment{T}"/> as <see cref="ReadOnlySequenceSegment{T}.Next"/> of <paramref name="previous"/> <see cref="MemorySequenceSegment{T}"/>
		/// </summary>
		/// <param name="previous">Previous <see cref="MemorySequenceSegment{T}"/>. This <see cref="MemorySequenceSegment{T}"/> is being set as <see cref="ReadOnlySequenceSegment{T}.Next"/> of <paramref name="previous"/> <see cref="MemorySequenceSegment{T}"/></param>
		internal void Reuse(MemorySequenceSegment<T> previous)
		{
			this.NextInternal = null;
			this.Next = null;
			this.Count = 0;
			this.RunningIndex = previous.RunningIndex + previous.Array.Length;

			previous.NextInternal = this;
			previous.Next = this;
		}

		/// <summary>
		/// Returns memory of this <see cref="MemorySequenceSegment{T}"/> to the shared pool
		/// </summary>
		public void Dispose()
		{
			var segment = Array;

			if (segment != null)
			{
				Memory = default;
				ArrayPool<T>.Shared.Return(segment, true);
				//This is set to null by design. From safety reason should not exist any referance to array after returned to pool. After Dispose segment is useless anyway.
#pragma warning disable CS8625
				Array = null;
#pragma warning restore CS8625
			}
		}
	}
}