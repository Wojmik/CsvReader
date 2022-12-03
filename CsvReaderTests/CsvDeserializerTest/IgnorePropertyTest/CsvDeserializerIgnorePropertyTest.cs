using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.TestItem;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.IgnorePropertyTest
{
	[TestClass]
	public class CsvDeserializerIgnorePropertyTest
	{
		static IEnumerable<object[]> GetSampleData()
		{
			yield return new object[]
			{
				@"Id,Text1,Text2,DayOfWeek,DayOfWeekNullable
7,Abcd,Efgh,Saturday,Friday
",
				new TestItem.TestItem[]
				{
					new TestItem.TestItem() { Id=7, Text1="Abcd", Text2=null, DayOfWeek=DayOfWeek.Saturday, DayOfWeekNullable=DayOfWeek.Friday, },
				}
			};

			yield return new object[]
			{
				@"Id,Text1,,DayOfWeek,DayOfWeekNullable
7,Abcd,Efgh,Saturday,Friday
",
				new TestItem.TestItem[]
				{
					new TestItem.TestItem() { Id=7, Text1="Abcd", Text2=null, DayOfWeek=DayOfWeek.Saturday, DayOfWeekNullable=DayOfWeek.Friday, },
				}
			};
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task IgnoreColumnTest(string sampleCsvContent, TestItem.TestItem[] expected)
		{
			List<TestItem.TestItem> actual;

			var recordConf = new RecordConfiguration_Text2_Ignored();

			using (var textReader = new StringReader(sampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, recordConfiguration: recordConf))
			{
				actual = await deserializer.ReadAllToListAsync();
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}

		class RecordConfiguration_Text2_Ignored : ICsvRecordTypeConfiguration<TestItem.TestItem>
		{
			public void Configure(RecordConfiguration<TestItem.TestItem> recordConfiguration)
			{
				recordConfiguration.Property(r => r.Text2)
					.Ignore();
			}
		}
	}
}