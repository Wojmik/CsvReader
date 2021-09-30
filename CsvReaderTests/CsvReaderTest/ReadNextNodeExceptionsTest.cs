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
			const string sample = "\"Test \"\"line\"\"\nsecond \"\"line\"\"";

			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=64, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, }))
			{
				await Assert.ThrowsExceptionAsync<SerializationException>(async () => await csvReader.ReadNextNodeAsMemorySequenceAsync(default));
			}
		}

		[TestMethod]
		public async Task UnexpectedCharAfterEscapedCellTestAsync()
		{
			const string sample = "\"Test \"\"line\"\"\nsecond \"\"line\"\"\"a";

			using(var textReader = new StringReader(sample))
			using(var csvReader = new CsvReader.CsvReader(textReader, new CsvReaderOptions() { BufferSizeInChars=64, CanEscape=true, DelimiterChar=',', EscapeChar='\"', LineEnding=LineEnding.Auto, }))
			{
				await Assert.ThrowsExceptionAsync<SerializationException>(async () => await csvReader.ReadNextNodeAsMemorySequenceAsync(default));
			}
		}
	}
}