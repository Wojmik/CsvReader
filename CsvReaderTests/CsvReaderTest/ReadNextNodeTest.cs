using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReader.CsvNodes;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReaderTests.CsvReaderTest
{
	[TestClass]
	public class ReadNextNodeTest
	{
		static IEnumerable<object[]> GetSampleData()
		{
			yield return new object[]
			{
				"",
				new StringNode[]
				{
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\r\n",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				",",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				",\r\n",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				",\r\n,",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				",\r\n,\r\n",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"A",
				new StringNode[]
				{
					new StringNode("A", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"A\r\n",
				new StringNode[]
				{
					new StringNode("A", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"A,B",
				new StringNode[]
				{
					new StringNode("A", NodeType.Cell),
					new StringNode("B", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"A,B\r\n",
				new StringNode[]
				{
					new StringNode("A", NodeType.Cell),
					new StringNode("B", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"A,B\r\nC,D",
				new StringNode[]
				{
					new StringNode("A", NodeType.Cell),
					new StringNode("B", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("C", NodeType.Cell),
					new StringNode("D", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"A,B\r\nC,D\r\n",
				new StringNode[]
				{
					new StringNode("A", NodeType.Cell),
					new StringNode("B", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("C", NodeType.Cell),
					new StringNode("D", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\"\"",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\"\"\r\n",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\"\",",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				",\"\"",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\"\",\"\"",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\"\",\"\"\r\n",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\"\",\"\"\r\n\"\"",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\"\",\"\"\r\n\"\",\"\"",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\"\",\"\"\r\n\"\",\"\"\r\n",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
			yield return new object[]
			{
				"\"\",\"\"\r\n\"\",\"\"\r\n\"\"",
				new StringNode[]
				{
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
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
			yield return new object[]
			{
				"Id,Text1,Text2\r\n7,Abcd,Efgh\r\n8,,\r\n9,,",
				new StringNode[]
				{
					new StringNode("Id", NodeType.Cell),
					new StringNode("Text1", NodeType.Cell),
					new StringNode("Text2", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("7", NodeType.Cell),
					new StringNode("Abcd", NodeType.Cell),
					new StringNode("Efgh", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("8", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("\r\n", NodeType.NewLine),
					new StringNode("9", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.Cell),
					new StringNode("", NodeType.EndOfStream),
					new StringNode("", NodeType.EndOfStream),
				}
			};
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task ReadNextNodeAsMemorySequenceBigBufferTestAsync(string sample, IEnumerable<StringNode> expectedNodes)
		{
			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=64, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, PermitEmptyLineAtEnd=true, }))
			{
				foreach(var expectedNode in expectedNodes)
				{
					var msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
					char[] actual = new char[msn.MemorySequence.CharsCount];
					msn.MemorySequence.CopyDataTo(actual);
					Assert.IsTrue(MemoryExtensions.SequenceEqual(actual, expectedNode.Data.AsSpan()));
				}
			}
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task ReadNextNodeAsMemorySequenceSmallBufferTestAsync(string sample, IEnumerable<StringNode> expectedNodes)
		{
			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=4, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, PermitEmptyLineAtEnd=true, }))
			{
				foreach(var expectedNode in expectedNodes)
				{
					var msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
					char[] actual = new char[msn.MemorySequence.CharsCount];
					msn.MemorySequence.CopyDataTo(actual);
					Assert.IsTrue(MemoryExtensions.SequenceEqual(actual, expectedNode.Data.AsSpan()));
				}
			}
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task ReadNextNodeAsMemoryBigBufferTestAsync(string sample, IEnumerable<StringNode> expectedNodes)
		{
			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=64, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, PermitEmptyLineAtEnd=true, }))
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
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=4, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, PermitEmptyLineAtEnd=true, }))
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
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=64, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, PermitEmptyLineAtEnd=true, }))
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
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=4, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, PermitEmptyLineAtEnd=true, }))
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