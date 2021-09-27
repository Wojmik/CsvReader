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
	static class CsvReaderExtensions
	{
#pragma warning disable CS8605
#pragma warning disable CS8602
		public static ValueTask<MemorySequenceSegmentSpan<char>> ReadChunkAsync(this CsvReader.CsvReader @this, CancellationToken cancellationToken)
		{
			var memberInfo = typeof(CsvReader.CsvReader).GetMethod("ReadChunkAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (ValueTask<MemorySequenceSegmentSpan<char>>)memberInfo.Invoke(@this, new object[] { cancellationToken, });
		}

		public static ValueTask<ReadCharResult> GetCharAsync(this CsvReader.CsvReader @this, MemorySequencePosition<char> currentPosition, int offset, CancellationToken cancellationToken)
		{
			var memberInfo = typeof(CsvReader.CsvReader).GetMethod("GetCharAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (ValueTask<ReadCharResult>)memberInfo.Invoke(@this, new object[] { currentPosition, offset, cancellationToken, });
		}

		public static ValueTask<ReadCharResult> FindCharAsync(this CsvReader.CsvReader @this, MemorySequencePosition<char> currentPosition, int offset, ReadOnlyMemory<char> charsToFind, CancellationToken cancellationToken)
		{
			var memberInfo = typeof(CsvReader.CsvReader).GetMethod("FindCharAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (ValueTask<ReadCharResult>)memberInfo.Invoke(@this, new object[] { currentPosition, offset, charsToFind, cancellationToken, });
		}

		public static MemorySequence<char> CharMemorySequence_Get(this CsvReader.CsvReader @this)
		{
			var memberInfo = typeof(CsvReader.CsvReader).GetField("CharMemorySequence", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (MemorySequence<char>)memberInfo.GetValue(@this);
		}

		public static Memory<char> SearchArray_Get(this CsvReader.CsvReader @this)
		{
			var memberInfo = typeof(CsvReader.CsvReader).GetField("SearchArray", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (Memory<char>)memberInfo.GetValue(@this);
		}

		public static ValueTask<bool> IsProperNewLineAsync(this CsvReader.CsvReader @this, ReadCharResult charRead, CancellationToken cancellationToken)
		{
			var memberInfo = typeof(CsvReader.CsvReader).GetMethod("IsProperNewLineAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (ValueTask<bool>)memberInfo.Invoke(@this, new object[] { charRead, cancellationToken, });
		}
#pragma warning restore CS8602
#pragma warning restore CS8605
	}
}