using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;

namespace WojciechMikołajewicz.CsvReaderTests.CsvReaderTest
{
	[TestClass]
	public class ReadNextNodeAsMemoryAndStringTest
	{
		static IEnumerable<object[]> GetSampleData()
		{
			yield return new object[]
			{
				",Abc,\"Test \"\"line\"\"\nsecond \"\"line\"\"\"\r\nCde",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("Abc", NodeType.Cell),
					new StringNode("Test \"line\"\nsecond \"line\"", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("Cde", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task ReadNextNodeAsMemoryBigBufferTestAsync(string sample, IEnumerable<StringNode> expectedNodes)
		{
			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=64, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, }))
			{
				foreach(var expectedNode in expectedNodes)
				{
					var mn = await csvReader.ReadNextNodeAsMemoryAsync(default);
					Assert.AreEqual(expectedNode.NodeType, mn.NodeType);
					Assert.IsTrue(MemoryExtensions.SequenceEqual(mn.Data.Span, expectedNode.Data.AsSpan()));
				}
			}
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task ReadNextNodeAsMemorySmallBufferTestAsync(string sample, IEnumerable<StringNode> expectedNodes)
		{
			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=4, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, }))
			{
				foreach(var expectedNode in expectedNodes)
				{
					var mn = await csvReader.ReadNextNodeAsMemoryAsync(default);
					Assert.AreEqual(expectedNode.NodeType, mn.NodeType);
					Assert.IsTrue(MemoryExtensions.SequenceEqual(mn.Data.Span, expectedNode.Data.AsSpan()));
				}
			}
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task ReadNextNodeAsStringBigBufferTestAsync(string sample, IEnumerable<StringNode> expectedNodes)
		{
			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=64, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, }))
			{
				foreach(var expectedNode in expectedNodes)
				{
					var sn = await csvReader.ReadNextNodeAsStringAsync(default);
					Assert.AreEqual(expectedNode.NodeType, sn.NodeType);
					Assert.IsTrue(string.Equals(sn.Data, expectedNode.Data));
				}
			}
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task ReadNextNodeAsStringSmallBufferTestAsync(string sample, IEnumerable<StringNode> expectedNodes)
		{
			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=4, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, }))
			{
				foreach(var expectedNode in expectedNodes)
				{
					var sn = await csvReader.ReadNextNodeAsStringAsync(default);
					Assert.AreEqual(expectedNode.NodeType, sn.NodeType);
					Assert.IsTrue(string.Equals(sn.Data, expectedNode.Data));
				}
			}
		}
	}
}