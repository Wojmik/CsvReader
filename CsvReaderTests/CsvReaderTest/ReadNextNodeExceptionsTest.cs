using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;

namespace WojciechMikołajewicz.CsvReaderTests.CsvReaderTest
{
	[TestClass]
	public class ReadNextNodeExceptionsTest
	{
		[TestMethod]
		public async Task EndOfStreamInsideEscapedCellTestAsync()
		{
			const string Sample = "\"Test \"\"line\"\"\nsecond \"\"line\"\"";

			using(var textReader = new StringReader(Sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, options =>
			{
				options.BufferSizeInChars = 64;
				options.CanEscape = true;
				options.DelimiterChar = ',';
				options.EscapeChar = '\"';
				options.LineEnding = LineEnding.Auto;
				}))
			{
				await Assert.ThrowsExceptionAsync<SerializationException>(async () => await csvReader.ReadNextNodeAsMemorySequenceAsync(default));
			}
		}

		[TestMethod]
		public async Task UnexpectedCharAfterEscapedCellTestAsync()
		{
			const string Sample = "\"Test \"\"line\"\"\nsecond \"\"line\"\"\"a";

			using(var textReader = new StringReader(Sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, options =>
			{
				options.BufferSizeInChars = 64;
				options.CanEscape = true;
				options.DelimiterChar = ',';
				options.EscapeChar = '\"';
				options.LineEnding = LineEnding.Auto;
				}))
			{
				await Assert.ThrowsExceptionAsync<SerializationException>(async () => await csvReader.ReadNextNodeAsMemorySequenceAsync(default));
			}
		}
	}
}