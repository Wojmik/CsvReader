using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.TestItem;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.StringDeduplicationTest
{
	[TestClass]
	public class StringDeduplicationTest
	{
		const string SampleCsvContent = @"Id,Text1,Text2,DayOfWeek,DayOfWeekNullable
7,Abcd,Abcd,Saturday,Friday
";

		public async Task StringDeduplicationEnabledTest()
		{
			var expected = new TestItem.TestItem[]
			{
				new TestItem.TestItem() { Id = 7, Text1 = "Abcd", Text2 = "Abcd", DayOfWeek = DayOfWeek.Saturday, DayOfWeekNullable = DayOfWeek.Friday, },
			};

			List<TestItem.TestItem> actual;

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
			{
				options.DeduplicateStrings = true;
			}))
			{
				actual = await deserializer.ReadAllToListAsync();
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
			Assert.AreSame(actual[0].Text1, actual[0].Text2);
		}

		public async Task StringDeduplicationDisabledTest()
		{
			var expected = new TestItem.TestItem[]
			{
				new TestItem.TestItem() { Id = 7, Text1 = "Abcd", Text2 = "Abcd", DayOfWeek = DayOfWeek.Saturday, DayOfWeekNullable = DayOfWeek.Friday, },
			};

			List<TestItem.TestItem> actual;

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
			{
				options.DeduplicateStrings = false;
			}))
			{
				actual = await deserializer.ReadAllToListAsync();
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
			Assert.AreNotSame(actual[0].Text1, actual[0].Text2);
		}
	}
}