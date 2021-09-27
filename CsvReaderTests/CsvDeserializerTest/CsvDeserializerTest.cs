using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.InternalModel;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest
{
	[TestClass]
	public class CsvDeserializerTest
	{
		static IEnumerable<object[]> GetSampleData()
		{
			yield return new object[]
			{
				@"Id,Text1,Text2
7,Abcd,Efgh
8,,
",
				new TestItem[]
				{
					new TestItem(){ Id=7, Text1="Abcd", Text2="Efgh", },
#pragma warning disable CS8625
					new TestItem(){ Id=8, Text1=null, Text2=null, },
#pragma warning restore CS8625
				}
			};
		}

#if NETCOREAPP3_0_OR_GREATER
		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public async Task DeserializeTestAsync(string sampleCsvContent, TestItem[] expected)
		{
			var actual = new List<TestItem>();

			var recordConf = new RecordConfigurator();

			using(var textReader = new StringReader(sampleCsvContent))
			using(var deserializer = new CsvDeserializer<TestItem>(textReader, recordConfiguration: recordConf))
			{
				await foreach(var item in deserializer.ReadAsync())
					actual.Add(item);
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}
#endif

		[DataTestMethod]
		[DynamicData(nameof(GetSampleData), DynamicDataSourceType.Method)]
		public void DeserializeTest(string sampleCsvContent, TestItem[] expected)
		{
			var actual = new List<TestItem>();

			var recordConf = new RecordConfigurator();

			using(var textReader = new StringReader(sampleCsvContent))
			using(var deserializer = new CsvDeserializer<TestItem>(textReader, recordConfiguration: recordConf))
			{
				foreach(var item in deserializer.Read())
					actual.Add(item);
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}

		class RecordConfigurator : ICsvRecordTypeConfiguration<TestItem>
		{
			public void Configure(RecordConfiguration<TestItem> recordConfiguration)
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