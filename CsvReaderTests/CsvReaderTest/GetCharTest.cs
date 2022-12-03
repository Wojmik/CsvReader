using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReader.MemorySequence;
using WojciechMikołajewicz.CsvReaderTests.TestDevices;

namespace WojciechMikołajewicz.CsvReaderTests.CsvReaderTest
{
	[TestClass]
	public class GetCharTest
	{
		const string TestString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		[TestMethod]
		public async Task GetCharTestAsync()
		{
			const int ChunkLength = 10;

			using(var textReader = new RepeatedTextReader(TestString) { MaxReadSize = ChunkLength, })
			using(var csvReader = new CsvReader.CsvReader(textReader, options =>
			{
				options.BufferSizeInChars = 32;
			}))
			{
				var memSeq = csvReader.CharMemorySequence_Get();

				//Read first char
				var firstChar = await csvReader.GetCharAsync(memSeq.CurrentPosition, 0, default);
				Assert.AreEqual(false, firstChar.EndOfStream);
				Assert.AreEqual(TestString[0 % TestString.Length], firstChar.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 0), firstChar.FoundPosition);

				//Read first char again
				var firstChar2 = await csvReader.GetCharAsync(firstChar.FoundPosition, 0, default);
				Assert.AreEqual(false, firstChar2.EndOfStream);
				Assert.AreEqual(TestString[0 % TestString.Length], firstChar2.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 0), firstChar2.FoundPosition);

				//Read second char
				var secondChar = await csvReader.GetCharAsync(firstChar2.FoundPosition, 1, default);
				Assert.AreEqual(false, secondChar.EndOfStream);
				Assert.AreEqual(TestString[1 % TestString.Length], secondChar.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 1), secondChar.FoundPosition);

				//Read second char again
				var secondChar2 = await csvReader.GetCharAsync(secondChar.FoundPosition, 0, default);
				Assert.AreEqual(false, secondChar2.EndOfStream);
				Assert.AreEqual(TestString[1 % TestString.Length], secondChar2.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 1), secondChar2.FoundPosition);

				//Read 9th char
				var ninethChar = await csvReader.GetCharAsync(secondChar2.FoundPosition, 8, default);
				Assert.AreEqual(false, ninethChar.EndOfStream);
				Assert.AreEqual(TestString[9 % TestString.Length], ninethChar.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 9), ninethChar.FoundPosition);

				//Read 9th char again
				var ninethChar2 = await csvReader.GetCharAsync(ninethChar.FoundPosition, 0, default);
				Assert.AreEqual(false, ninethChar2.EndOfStream);
				Assert.AreEqual(TestString[9 % TestString.Length], ninethChar2.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 9), ninethChar2.FoundPosition);

				//Read 10th char
				var tentthChar = await csvReader.GetCharAsync(ninethChar.FoundPosition, 1, default);
				Assert.AreEqual(false, tentthChar.EndOfStream);
				Assert.AreEqual(TestString[10 % TestString.Length], tentthChar.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 10), tentthChar.FoundPosition);

				//Read 10th char again
				var tentthChar2 = await csvReader.GetCharAsync(tentthChar.FoundPosition, 0, default);
				Assert.AreEqual(false, tentthChar2.EndOfStream);
				Assert.AreEqual(TestString[10 % TestString.Length], tentthChar2.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 10), tentthChar2.FoundPosition);
			}
		}

		[TestMethod]
		public async Task GetCharStartFromThirdTestAsync()
		{
			const int ChunkLength = 10;

			using(var textReader = new RepeatedTextReader(TestString) { MaxReadSize = ChunkLength, })
			using(var csvReader = new CsvReader.CsvReader(textReader, options =>
			{
				options.BufferSizeInChars = 32;
			}))
			{
				var memSeq = csvReader.CharMemorySequence_Get();

				//Read third char
				var thirdChar = await csvReader.GetCharAsync(memSeq.CurrentPosition, 3, default);
				Assert.AreEqual(false, thirdChar.EndOfStream);
				Assert.AreEqual(TestString[3 % TestString.Length], thirdChar.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 3), thirdChar.FoundPosition);

				//Read third char again
				var thirdChar2 = await csvReader.GetCharAsync(thirdChar.FoundPosition, 0, default);
				Assert.AreEqual(false, thirdChar2.EndOfStream);
				Assert.AreEqual(TestString[3 % TestString.Length], thirdChar2.Character);
				Assert.AreEqual(new MemorySequencePosition<char>(memSeq.CurrentPosition.InternalSequenceSegment, 3), thirdChar2.FoundPosition);
			}
		}

		[TestMethod]
		public async Task GetCharBigJumpTestAsync()
		{
			const int ChunkLength = 10;

			using(var textReader = new RepeatedTextReader(TestString) { MaxReadSize = ChunkLength, })
			using(var csvReader = new CsvReader.CsvReader(textReader, options =>
			{
				options.BufferSizeInChars = 32;
			}))
			{
				var memSeq = csvReader.CharMemorySequence_Get();

				//Read at long jump
				var jumpChar = await csvReader.GetCharAsync(memSeq.CurrentPosition, 1052, default);
				Assert.AreEqual(false, jumpChar.EndOfStream);
				Assert.AreEqual(TestString[1052 % TestString.Length], jumpChar.Character);
				Assert.AreEqual(1052, jumpChar.FoundPosition.AbsolutePosition);

				//Read at long jump again
				var jumpChar2 = await csvReader.GetCharAsync(jumpChar.FoundPosition, 0, default);
				Assert.AreEqual(false, jumpChar2.EndOfStream);
				Assert.AreEqual(TestString[1052 % TestString.Length], jumpChar2.Character);
				Assert.AreEqual(1052, jumpChar2.FoundPosition.AbsolutePosition);
			}
		}
	}
}