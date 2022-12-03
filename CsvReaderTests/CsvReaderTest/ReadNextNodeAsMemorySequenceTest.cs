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
	public class ReadNextNodeAsMemorySequenceTest
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
					new StringNode("Test \"\"line\"\"\nsecond \"\"line\"\"", NodeType.Cell),
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
			using(var csvReader = new CsvReader.CsvReader(textReader, options =>
			{
				options.BufferSizeInChars = 64;
				options.CanEscape = true;
				options.DelimiterChar = ',';
				options.EscapeChar = '\"';
				options.LineEnding = LineEnding.Auto;
				options.PermitEmptyLineAtEnd = true;
			}))
			{
				foreach(var expectedNode in expectedNodes)
				{
					var msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
					CheckNodeText(expectedNode.NodeType, expectedNode.Data, msn);
				}
			}
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task ReadNextNodeAsMemorySequenceSmallBufferTestAsync(string sample, IEnumerable<StringNode> expectedNodes)
		{
			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, options =>
			{
				options.BufferSizeInChars = 4;
				options.CanEscape = true;
				options.DelimiterChar = ',';
				options.EscapeChar = '\"';
				options.LineEnding = LineEnding.Auto;
				options.PermitEmptyLineAtEnd = true;
				}))
			{
				foreach(var expectedNode in expectedNodes)
				{
					var msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
					CheckNodeText(expectedNode.NodeType, expectedNode.Data, msn);
				}
			}
		}

		private void CheckNodeText(NodeType expectedNodeType, string expected, MemorySequenceNode actual)
		{
			ReadOnlySpan<char> expectedChunk, actualChunk;

			Assert.AreEqual(expectedNodeType, actual.NodeType);
			//Assert.AreEqual(escapePositions.Length, actual.SkipCharPositions.Count);

			//Compare chunks
			var segment = actual.MemorySequence.StartPosition.SequenceSegment;
			int expectedIndex = 0, actualIndex = actual.MemorySequence.StartPosition.PositionInSegment;
			while(!ReferenceEquals(segment, actual.MemorySequence.EndPosition.SequenceSegment))
			{
				expectedChunk = expected.AsSpan(expectedIndex, segment.Memory.Length-actualIndex);
				actualChunk = segment.Memory.Slice(actualIndex).Span;

				Assert.IsTrue(MemoryExtensions.SequenceEqual(actualChunk, expectedChunk));

				segment = segment.Next
#if NETCOREAPP3_0_OR_GREATER
					!
#endif
					;
				actualIndex = 0;
				expectedIndex += expectedChunk.Length;
			}

			//Compare last chunk
			expectedChunk = expected.AsSpan(expectedIndex, expected.Length-expectedIndex);
			actualChunk = segment.Memory.Slice(actualIndex, actual.MemorySequence.EndPosition.PositionInSegment-actualIndex).Span;
			Assert.IsTrue(MemoryExtensions.SequenceEqual(actualChunk, expectedChunk));

			//Check skip positions
			int skipCharIndex = 0, skipListIndex = 0, positionInSegment;
			long absoluteStartCellPosition = actual.MemorySequence.StartPosition.AbsolutePosition;
			while(0<=(skipCharIndex=expected.IndexOf("\"\"", skipCharIndex)))
			{
				var skipEntry = actual.MemorySequence.SkipCharPositions[skipListIndex];
				Assert.AreEqual('\"', skipEntry.SequenceSegment.Memory.Span[skipEntry.PositionInSegment]);

				int relativePosition = (int)(skipEntry.AbsolutePosition-absoluteStartCellPosition);
				Assert.IsTrue(skipCharIndex==relativePosition || skipCharIndex+1==relativePosition);

				bool corectPosition = false;
				segment = actual.MemorySequence.StartPosition.SequenceSegment;
				positionInSegment = actual.MemorySequence.StartPosition.PositionInSegment + skipCharIndex;
				while(segment.Memory.Length<=positionInSegment)
				{
					positionInSegment -= segment.Memory.Length;
					segment = segment.Next
#if NETCOREAPP3_0_OR_GREATER
						!
#endif
						;
				}
				corectPosition |= new MemorySequencePosition<char>((MemorySequenceSegment<char>)segment, positionInSegment)==skipEntry;

				positionInSegment++;
				while(segment.Memory.Length<=positionInSegment)
				{
					positionInSegment -= segment.Memory.Length;
					segment = segment.Next
#if NETCOREAPP3_0_OR_GREATER
						!
#endif
						;
				}
				corectPosition |= new MemorySequencePosition<char>((MemorySequenceSegment<char>)segment, positionInSegment)==skipEntry;

				Assert.IsTrue(corectPosition);

				skipCharIndex += 2;
				skipListIndex++;
			}
			Assert.AreEqual(skipListIndex, actual.MemorySequence.SkipCharPositions.Count);
		}
	}
}