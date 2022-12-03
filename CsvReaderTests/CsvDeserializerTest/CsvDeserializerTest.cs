using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.TestItem;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest
{
	[TestClass]
	public class CsvDeserializerTest
	{
		static IEnumerable<object[]> GetSampleData()
		{
			yield return new object[]
			{
				@"Id,Text1,Text2,DayOfWeek,DayOfWeekNullable
7,Abcd,Efgh,Saturday,Friday
8,,,1,
",
				new TestItem.TestItem[]
				{
					new TestItem.TestItem() { Id=7, Text1="Abcd", Text2="Efgh", DayOfWeek=DayOfWeek.Saturday, DayOfWeekNullable=DayOfWeek.Friday, },
#pragma warning disable CS8625
					new TestItem.TestItem() { Id=8, Text1=null, Text2=null, DayOfWeek=(DayOfWeek)1, DayOfWeekNullable=null, },
#pragma warning restore CS8625
				}
			};
		}

#if NETCOREAPP3_0_OR_GREATER
		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task DeserializeTestAsync(string sampleCsvContent, TestItem.TestItem[] expected)
		{
			var actual = new List<TestItem.TestItem>();

			var recordConf = new RecordConfigurator();

			using (var textReader = new StringReader(sampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, recordConfiguration: recordConf))
			{
				await foreach (var item in deserializer.ReadAsync())
					actual.Add(item);
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}
#endif

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task DeserializeToListTestAsync(string sampleCsvContent, TestItem.TestItem[] expected)
		{
			List<TestItem.TestItem> actual;

			var recordConf = new RecordConfigurator();

			using (var textReader = new StringReader(sampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, recordConfiguration: recordConf))
			{
				actual = await deserializer.ReadAllToListAsync();
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public void DeserializeTest(string sampleCsvContent, TestItem.TestItem[] expected)
		{
			var actual = new List<TestItem.TestItem>();

			var recordConf = new RecordConfigurator();

			using (var textReader = new StringReader(sampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, recordConfiguration: recordConf))
			{
				foreach (var item in deserializer.Read())
					actual.Add(item);
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public void DeserializeToListTest(string sampleCsvContent, TestItem.TestItem[] expected)
		{
			List<TestItem.TestItem> actual;

			var recordConf = new RecordConfigurator();

			using (var textReader = new StringReader(sampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, recordConfiguration: recordConf))
			{
				actual = deserializer.ReadAllToList();
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}

		class RecordConfigurator : ICsvRecordTypeConfiguration<TestItem.TestItem>
		{
			public void Configure(RecordConfiguration<TestItem.TestItem> recordConfiguration)
			{
				//recordConfiguration
				//	.Property(rec => rec.Text)
				//	.BindToColumn(nameof(TestItem.Text))
				//	.ConfigureDeserializer(deserializer =>
				//	{
				//	});
				//recordConfiguration
				//	.Property(bec => bec.Text)
				//	.BindToColumn(nameof(TestItem.Text))
				//	.ConfigureDeserializer(deserializer =>
				//	{
				//	});

				//recordConfiguration
				//	.Property(rec => rec.Id)
				//	.BindToColumn(nameof(TestItem.Id))
				//	.ConfigureDeserializer(deserializer =>
				//	{
				//	});
			}
		}
	}
}