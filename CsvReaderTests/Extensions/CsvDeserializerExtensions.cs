using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReaderTests
{
	static class CsvDeserializerExtensions
	{
		public static ValueTask<MemorySequenceSegmentSpan<char>> ReadChunkAsync(this CsvDeserializer @this, CancellationToken cancellationToken)
		{
			var memberInfo = typeof(CsvDeserializer).GetMethod("ReadChunkAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (ValueTask<MemorySequenceSegmentSpan<char>>)memberInfo.Invoke(@this, new object[] { cancellationToken, });
		}

		public static ValueTask<ReadCharResult> GetCharAsync(this CsvDeserializer @this, MemorySequencePosition<char> currentPosition, int offset, CancellationToken cancellationToken)
		{
			var memberInfo = typeof(CsvDeserializer).GetMethod("GetCharAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (ValueTask<ReadCharResult>)memberInfo.Invoke(@this, new object[] { currentPosition, offset, cancellationToken, });
		}

		public static ValueTask<ReadCharResult> FindCharAsync(this CsvDeserializer @this, MemorySequencePosition<char> currentPosition, int offset, ReadOnlyMemory<char> charsToFind, CancellationToken cancellationToken)
		{
			var memberInfo = typeof(CsvDeserializer).GetMethod("FindCharAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (ValueTask<ReadCharResult>)memberInfo.Invoke(@this, new object[] { currentPosition, offset, charsToFind, cancellationToken, });
		}

		public static MemorySequence<char> CharMemorySequence_Get(this CsvDeserializer @this)
		{
			var memberInfo = typeof(CsvDeserializer).GetField("CharMemorySequence", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (MemorySequence<char>)memberInfo.GetValue(@this);
		}
	}
}