using WojciechMikołajewicz.CsvReaderTests.TestDevices;
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
	public class ReadChunkTest
	{
		const string TestString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		[TestMethod]
		public async Task TestAsync()
		{
			using(var textReader = new StringReader(TestString))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars = 32, }))
			{
				Assert.AreEqual(0, csvReader.Position, "Position should be 0 and is {0}", csvReader.Position);

				MemorySequenceSegmentSpan<char> segmentRead;
				int testStringPosition = 0;

				//Check Loads
				while((segmentRead=await csvReader.ReadChunkAsync(default)).Length>0)
				{
					Assert.AreEqual(0, csvReader.Position, "Position should be 0 and is {0}", csvReader.Position);

					Assert.IsTrue(TestString.AsSpan(testStringPosition, segmentRead.Length).SequenceEqual(segmentRead.Memory.Span));
					testStringPosition+=segmentRead.Length;
				}

				//Are all chars read
				Assert.AreEqual(TestString.Length, testStringPosition, "Should read {0} chars and read {1}", TestString.Length, testStringPosition);

				//Check sequence
				testStringPosition = 0;
				var segment = csvReader.CharMemorySequence_Get().CurrentPosition.InternalSequenceSegment;
				while(segment!=null && segment.Count>0)
				{
					Assert.IsTrue(TestString.AsSpan(testStringPosition, segment.Count).SequenceEqual(segment.Memory.Span));
					testStringPosition+=segment.Count;

					segment = segment.NextInternal;
				}

				//Are all chars read
				Assert.AreEqual(TestString.Length, testStringPosition, "Should read {0} chars and read {1}", TestString.Length, testStringPosition);

				//Read chunk after EOF
				var segmentEOF = await csvReader.ReadChunkAsync(default);
				Assert.AreEqual(0, segmentEOF.Length, "Read after end of stream should be empty");
				Assert.AreEqual(0, csvReader.Position, "Position should be 0 and is {0}", csvReader.Position);
			}
		}

		[TestMethod]
		public async Task TestSmallReadsAsync()
		{
			const int ChunkLength = 10;
			int sequenceSwitch = 0, readerPosition = 0;
			MemorySequenceSegmentSpan<char> segmentRead = default;
			MemorySequenceSegment<char> previousSegment = null;

			using(var textReader = new RepeatedTextReader(TestString) { MaxReadSize = ChunkLength, })
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars = 32, }))
			{
				while(sequenceSwitch<3)
				{
					previousSegment = segmentRead.Segment;

					segmentRead = await csvReader.ReadChunkAsync(default);

					if(!Object.ReferenceEquals(previousSegment?.Array, segmentRead.Segment.Array))
						sequenceSwitch++;

					//Check chunk
					for(int i = 0; i<segmentRead.Length; i++, readerPosition++)
						Assert.AreEqual(textReader.RepeatedText[readerPosition % textReader.RepeatedText.Length], segmentRead.Memory.Span[i], "Character at position {0} should be '{1}' and is '{2}'", i, textReader.RepeatedText[readerPosition % textReader.RepeatedText.Length], segmentRead.Memory.Span[i]);
				}

				textReader.Length = textReader.Position;

				//Read last chunk
				segmentRead = await csvReader.ReadChunkAsync(default);

				//Check chunk
				for(int i = 0; i<segmentRead.Length; i++, readerPosition++)
					Assert.AreEqual(textReader.RepeatedText[readerPosition % textReader.RepeatedText.Length], segmentRead.Memory.Span[i], "Character at position {0} should be '{1}' and is '{2}'", i, textReader.RepeatedText[readerPosition % textReader.RepeatedText.Length], segmentRead.Memory.Span[i]);

				//Should be EOF
				segmentRead = await csvReader.ReadChunkAsync(default);
				Assert.AreEqual(0, segmentRead.Length, "Should be end of stream");

				//Should be EOF again
				segmentRead = await csvReader.ReadChunkAsync(default);
				Assert.AreEqual(0, segmentRead.Length, "Should be end of stream");

				//Check whole sequence
				textReader.Position = 0;
				textReader.MaxReadSize = int.MaxValue;
				char[] expected = Array.Empty<char>();
				var segment = csvReader.CharMemorySequence_Get().CurrentPosition.InternalSequenceSegment;
				while(segment!=null && segment.Count>0)
				{
					//If buffer to small, reallocate
					if(expected.Length<segment.Count)
						expected = new char[segment.Count];

					if(segment.Count!=await textReader.ReadBlockAsync(expected.AsMemory(0, segment.Count)))
						throw new Exception("Buffer not fully loaded");

					Assert.IsTrue(expected.AsSpan(0, segment.Count).SequenceEqual(segment.Memory.Span));

					segment = segment.NextInternal;
				}
			}
		}

		[TestMethod]
		public async Task TestEmojiAtSegmentsBoundaryAsync()
		{
			const string emoji = "😎";

			using(var memoryStream = new MemoryStream())
			using(var streamWritter = new StreamWriter(memoryStream, Encoding.UTF8))
			using(var streamReader = new StreamReader(memoryStream, streamWritter.Encoding, true))
			using(var csvReader = new CsvReader.CsvReader(streamReader, new CsvReaderOptions() { BufferSizeInChars = 32, }))
			{
				MemorySequenceSegmentSpan<char> segmentRead = default;
				var segmentLength = csvReader.CharMemorySequence_Get().CurrentPosition.InternalSequenceSegment.Array.Length;
				int written = 0, pos, toWrite, read = 0;
				var sbExpected = new StringBuilder(128);

				//Save text for whole segment except last char
				while(written<segmentLength-1)
				{
					pos = written % TestString.Length;
					toWrite =  Math.Min(TestString.Length-pos, segmentLength-1-written);
					streamWritter.Write(TestString.AsSpan(pos, toWrite));
					sbExpected.Append(TestString.AsSpan(pos, toWrite));
					written += toWrite;
				}

				//Save emoji at the segments boundary
				streamWritter.Write(emoji);
				sbExpected.Append(emoji);

				//Write something else for the second segment
				pos = written % TestString.Length;
				toWrite = TestString.Length-pos;
				streamWritter.Write(TestString.AsSpan(pos, toWrite));
				sbExpected.Append(TestString.AsSpan(pos, toWrite));
				written += toWrite;

				streamWritter.Flush();
				memoryStream.Position = 0;
				var expected = sbExpected.ToString();

				//Read to segments
				while((segmentRead = await csvReader.ReadChunkAsync(default)).Length>0)
				{
					//Check read chunk
					Assert.IsTrue(expected.AsSpan(read, segmentRead.Length).SequenceEqual(segmentRead.Memory.Span));
					read += segmentRead.Length;
				}

				//Check whole sequence
				var segment = csvReader.CharMemorySequence_Get().CurrentPosition.InternalSequenceSegment;
				read = 0;
				while(segment!=null && segment.Count>0)
				{
					Assert.IsTrue(expected.AsSpan(read, segment.Count).SequenceEqual(segment.Memory.Span));
					read += segment.Count;

					segment = segment.NextInternal;
				}
			}
		}
	}
}