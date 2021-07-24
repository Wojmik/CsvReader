using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReaderTests.TestDevices;

namespace WojciechMikołajewicz.CsvReaderTests
{
	[TestClass]
	public class FindCharTest
	{
		const string TestString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ\"";

		[TestMethod]
		public async Task FindCharTestAsync()
		{
			const int ChunkLength = 10;

			using var textReader = new RepeatedTextReader(TestString) { MaxReadSize = ChunkLength, };
			using var csvReader = new CsvDeserializer(textReader, false, bufferSize: 32);

			var memSeq = csvReader.CharMemorySequence_Get();

			//Read third char again
			var foundChar = await csvReader.FindCharAsync(memSeq.CurrentPosition, 109, new char[] { '\"' }, default);
			Assert.AreEqual(false, foundChar.EndOfStream);

			int skipTimes = Math.DivRem(109, TestString.Length, out int startPosInTestString);
			int foundPosition = TestString.Length*skipTimes+(TestString+TestString).IndexOf('\"', startPosInTestString);

			Assert.AreEqual('\"', foundChar.Character);
			Assert.AreEqual(foundPosition, foundChar.FoundPosition.AbsolutePosition);
		}
	}
}