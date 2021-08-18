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
	public class IsProperNewLineTest
	{
		[DataTestMethod]
		[DataRow(LineEnding.Auto, "\r\n")]
		[DataRow(LineEnding.CRLF, "\r")]
		[DataRow(LineEnding.LF, "\n")]
		[DataRow(LineEnding.CR, "\r")]
		public void CheckSearchArray(LineEnding lineEnding, string newLineSearchCharacters)
		{
			using var textReader = new StringReader(string.Empty);
			var csvReaderOptions = new CsvReaderOptions() { LineEnding = lineEnding, BufferSizeInChars = 32, };
			using var csvReader = new CsvReader.CsvReader(textReader, csvReaderOptions);

			Assert.AreEqual(lineEnding, csvReader.LineEnding);

			var searchArray = csvReader.SearchArray_Get();

			Assert.AreEqual(newLineSearchCharacters.Length+1, searchArray.Length);

			Assert.IsTrue(searchArray.Span.Contains(csvReaderOptions.DelimiterChar));
			foreach(var ch in newLineSearchCharacters)
				Assert.IsTrue(searchArray.Span.Contains(ch));
		}

		[DataTestMethod]
		[DataRow(LineEnding.CRLF, "", false)]
		[DataRow(LineEnding.CRLF, "A", false)]
		[DataRow(LineEnding.CRLF, "\r", false)]
		[DataRow(LineEnding.CRLF, "\n", false)]
		[DataRow(LineEnding.CRLF, "\r\n", true)]
		[DataRow(LineEnding.LF, "", false)]
		[DataRow(LineEnding.LF, "A", false)]
		[DataRow(LineEnding.LF, "\r", false)]
		[DataRow(LineEnding.LF, "\n", true)]
		[DataRow(LineEnding.CR, "", false)]
		[DataRow(LineEnding.CR, "A", false)]
		[DataRow(LineEnding.CR, "\r", true)]
		[DataRow(LineEnding.CR, "\n", false)]
		public async Task IsProperNewLineTestAsync(LineEnding lineEnding, string sample, bool expected)
		{
			using var textReader = new StringReader(sample);
			using var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { LineEnding = lineEnding, BufferSizeInChars = 32, });

			var charRead = await csvReader.GetCharAsync(csvReader.CharMemorySequence_Get().CurrentPosition, 0, default);

			var actual = await csvReader.IsProperNewLineAsync(charRead, default);

			Assert.AreEqual(expected, actual);
		}

		[DataTestMethod]
		[DataRow("", false, LineEnding.Auto, "\r\n")]
		[DataRow("A", false, LineEnding.Auto, "\r\n")]
		[DataRow("\r", true, LineEnding.CR, "\r")]
		[DataRow("\rA", true, LineEnding.CR, "\r")]
		[DataRow("\n", true, LineEnding.LF, "\n")]
		[DataRow("\n\r", true, LineEnding.LF, "\n")]
		[DataRow("\nA", true, LineEnding.LF, "\n")]
		[DataRow("\r\n", true, LineEnding.CRLF, "\r")]
		[DataRow("\r\nA", true, LineEnding.CRLF, "\r")]
		public async Task AutoLineEndingTestAsync(string sample, bool expected, LineEnding afterLineEnding, string afterNewLineSearchCharacters)
		{
			using var textReader = new StringReader(sample);
			var csvReaderOptions = new CsvReaderOptions() { LineEnding = LineEnding.Auto, BufferSizeInChars = 32, };
			using var csvReader = new CsvReader.CsvReader(textReader, csvReaderOptions);

			var charRead = await csvReader.GetCharAsync(csvReader.CharMemorySequence_Get().CurrentPosition, 0, default);

			var actual = await csvReader.IsProperNewLineAsync(charRead, default);

			Assert.AreEqual(expected, actual);

			Assert.AreEqual(afterLineEnding, csvReader.LineEnding);

			var searchArray = csvReader.SearchArray_Get();

			Assert.AreEqual(afterNewLineSearchCharacters.Length+1, searchArray.Length);

			Assert.IsTrue(searchArray.Span.Contains(csvReaderOptions.DelimiterChar));
			foreach(var ch in afterNewLineSearchCharacters)
				Assert.IsTrue(searchArray.Span.Contains(ch));
		}
	}
}