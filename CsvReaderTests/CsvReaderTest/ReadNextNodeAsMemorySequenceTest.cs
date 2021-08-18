using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReaderTests.CsvReaderTest
{
	[TestClass]
	public class ReadNextNodeAsMemorySequenceTest
	{
		[TestMethod]
		public async Task ReadNextNodeAsMemorySequenceTestAsync()
		{
			const string testData = ",Abc,\"Test \"\"line\"\"\nsecond \"\"line\"\"\"\r\nCde";

			using var textReader = new StringReader(testData);
			using var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=4, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, });
			MemorySequenceNode msn;

			msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
			CheckNodeText(NodeType.Cell, "", msn);

			msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
			CheckNodeText(NodeType.Cell, "Abc", msn);

			msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
			CheckNodeText(NodeType.Cell, "Test \"\"line\"\"\nsecond \"\"line\"\"", msn);

			msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
			CheckNodeText(NodeType.NewLine, "\r\n", msn);

			msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
			CheckNodeText(NodeType.Cell, "Cde", msn);

			msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
			Assert.AreEqual(NodeType.EndOfStream, msn.NodeType);
			Assert.AreEqual(0, msn.SkipCharPositions.Count);

			msn = await csvReader.ReadNextNodeAsMemorySequenceAsync(default);
			Assert.AreEqual(NodeType.EndOfStream, msn.NodeType);
			Assert.AreEqual(0, msn.SkipCharPositions.Count);
		}

		private void CheckNodeText(NodeType expectedNodeType, string expected, MemorySequenceNode actual)
		{
			ReadOnlySpan<char> expectedChunk, actualChunk;

			Assert.AreEqual(expectedNodeType, actual.NodeType);
			//Assert.AreEqual(escapePositions.Length, actual.SkipCharPositions.Count);

			//Compare chunks
			var segment = actual.StartPosition.SequenceSegment;
			int expectedIndex = 0, actualIndex = actual.StartPosition.PositionInSegment;
			while(segment!=actual.EndPosition.SequenceSegment)
			{
				expectedChunk = expected.AsSpan(expectedIndex, segment.Memory.Length-actualIndex);
				actualChunk = segment.Memory.Slice(actualIndex).Span;

				Assert.IsTrue(MemoryExtensions.SequenceEqual(actualChunk, expectedChunk));

				segment = segment.Next;
				actualIndex = 0;
				expectedIndex += expectedChunk.Length;
			}

			//Compare last chunk
			expectedChunk = expected.AsSpan(expectedIndex, expected.Length-expectedIndex);
			actualChunk = segment.Memory.Slice(actualIndex, expected.Length-expectedIndex).Span;
			Assert.IsTrue(MemoryExtensions.SequenceEqual(actualChunk, expectedChunk));

			//Check skip positions
			int skipCharIndex = 0, skipListIndex = 0, positionInSegment;
			long absoluteStartCellPosition = actual.StartPosition.AbsolutePosition;
			while(0<=(skipCharIndex=expected.IndexOf("\"\"", skipCharIndex)))
			{
				var skipEntry = actual.SkipCharPositions[skipListIndex];
				Assert.AreEqual('\"', skipEntry.SequenceSegment.Memory.Span[skipEntry.PositionInSegment]);

				int relativePosition = (int)(skipEntry.AbsolutePosition-absoluteStartCellPosition);
				Assert.IsTrue(skipCharIndex==relativePosition || skipCharIndex+1==relativePosition);

				bool corectPosition = false;
				segment = actual.StartPosition.SequenceSegment;
				positionInSegment = actual.StartPosition.PositionInSegment + skipCharIndex;
				while(segment.Memory.Length<=positionInSegment)
				{
					positionInSegment -= segment.Memory.Length;
					segment = segment.Next;
				}
				corectPosition |= new MemorySequencePosition<char>((MemorySequenceSegment<char>)segment, positionInSegment)==skipEntry;

				positionInSegment++;
				while(segment.Memory.Length<=positionInSegment)
				{
					positionInSegment -= segment.Memory.Length;
					segment = segment.Next;
				}
				corectPosition |= new MemorySequencePosition<char>((MemorySequenceSegment<char>)segment, positionInSegment)==skipEntry;

				Assert.IsTrue(corectPosition);

				skipCharIndex += 2;
				skipListIndex++;
			}
			Assert.AreEqual(skipListIndex, actual.SkipCharPositions.Count);
		}
	}
}