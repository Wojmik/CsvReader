using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReader.CsvNodes;
using WojciechMikołajewicz.CsvReaderTests.TestDevices;

namespace WojciechMikołajewicz.CsvReaderTests.CsvReaderTest
{
	[TestClass]
	public class ReadNewLineTest
	{
		[TestMethod]
		public async Task ReadNewLineAtChunksBoundaryTest()
		{
			using(var textReader = new WritableTextReader())
			using(var csvReader = new CsvReader.CsvReader(textReader, options =>
			{
				options.BufferSizeInChars = 32;
				options.LineEnding = LineEnding.CRLF;
			}))
			{
				var firstChunkSize = csvReader.CharMemorySequence_Get().CurrentPosition.InternalSequenceSegment.Array.Length;
				var sample = string.Join(string.Empty, Enumerable.Range(0, firstChunkSize*5).Select(num => (num%10)));
				sample = sample.Substring(0, firstChunkSize-1) + "\r\n" + sample.Substring(firstChunkSize+1);
				textReader.WriteAllText(sample);

				var actual1 = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.Cell, actual1.NodeType);
				Assert.IsTrue(MemoryExtensions.Equals(sample.AsSpan(0, firstChunkSize-1), actual1.Data.AsSpan(), StringComparison.Ordinal));
				
				var actual2 = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.NewLine, actual2.NodeType);
				Assert.IsTrue(string.Equals("\r\n", actual2.Data, StringComparison.Ordinal));

				var actual3 = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.Cell, actual3.NodeType);
				Assert.IsTrue(MemoryExtensions.Equals(sample.AsSpan(firstChunkSize+1), actual3.Data.AsSpan(), StringComparison.Ordinal));

				var eof = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.EndOfStream, eof.NodeType);
				eof = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.EndOfStream, eof.NodeType);
			}
		}

		[TestMethod]
		public async Task ReadNewLineAtChunksBoundaryEofTest()
		{
			using(var textReader = new WritableTextReader())
			using(var csvReader = new CsvReader.CsvReader(textReader, options =>
			{
				options.BufferSizeInChars = 32;
				options.LineEnding = LineEnding.CRLF;
			}))
			{
				var firstChunkSize = csvReader.CharMemorySequence_Get().CurrentPosition.InternalSequenceSegment.Array.Length;
				var sample = string.Join(string.Empty, Enumerable.Range(0, firstChunkSize+2).Select(num => (num%10)));
				sample = sample.Substring(0, firstChunkSize-1) + "\r\n" + sample.Substring(firstChunkSize+1);
				textReader.WriteAllText(sample);

				var actual1 = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.Cell, actual1.NodeType);
				Assert.IsTrue(MemoryExtensions.Equals(sample.AsSpan(0, firstChunkSize-1), actual1.Data.AsSpan(), StringComparison.Ordinal));
				
				var actual2 = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.NewLine, actual2.NodeType);
				Assert.IsTrue(string.Equals("\r\n", actual2.Data, StringComparison.Ordinal));

				var actual3 = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.Cell, actual3.NodeType);
				Assert.IsTrue(MemoryExtensions.Equals(sample.AsSpan(firstChunkSize+1), actual3.Data.AsSpan(), StringComparison.Ordinal));

				var eof = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.EndOfStream, eof.NodeType);
				eof = await csvReader.ReadNextNodeAsStringAsync();
				Assert.AreEqual(NodeType.EndOfStream, eof.NodeType);
			}
		}
	}
}