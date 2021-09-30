using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;

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
				new TestItem[]
				{
					new TestItem(){ Id=7, Text1="Abcd", Text2="Efgh", DayOfWeek=DayOfWeek.Saturday, DayOfWeekNullable=DayOfWeek.Friday, },
#pragma warning disable CS8625
					new TestItem(){ Id=8, Text1=null, Text2=null, DayOfWeek=(DayOfWeek)1, DayOfWeekNullable=null, },
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

		public class TestItem
		{
			public int Id { get; set; }

#pragma warning disable CS8618
			public string Text1 { get; set; }
#pragma warning restore CS8618

#if NETCOREAPP3_0_OR_GREATER
			public string? Text2 { get; set; }
#else
			public string Text2 { get; set; }
#endif

			public DayOfWeek DayOfWeek { get; set; }

			public DayOfWeek? DayOfWeekNullable { get; set; }
		}

		class TestItemEqualityComparer : IEqualityComparer<TestItem>
		{
#if NETCOREAPP3_0_OR_GREATER
			public bool Equals(TestItem? x, TestItem? y)
#else
			public bool Equals(TestItem x, TestItem y)
#endif
			{
				return
					(x==null && y==null) ||
					(x!=null && y!=null
					&& x.Id==y.Id
					&& x.Text1==y.Text1
					&& x.Text2==y.Text2
					&& x.DayOfWeek==y.DayOfWeek
					&& Nullable.Equals(x.DayOfWeekNullable, y.DayOfWeekNullable)
					);
			}

#if NETCOREAPP3_0_OR_GREATER
			public int GetHashCode(TestItem? obj)
#else
			public int GetHashCode(TestItem obj)
#endif
			{
				int hashCode = -1219660215;
				if(obj!=null)
				{
					hashCode=hashCode*-1521134295+obj.Id.GetHashCode();
					hashCode=hashCode*-1521134295+EqualityComparer<string>.Default.GetHashCode(obj.Text1);
#pragma warning disable CS8604
					hashCode=hashCode*-1521134295+EqualityComparer<string>.Default.GetHashCode(obj.Text2);
#pragma warning restore CS8604
					hashCode=hashCode*-1521134295+obj.DayOfWeek.GetHashCode();
					hashCode=hashCode*-1521134295+obj.DayOfWeekNullable.GetHashCode();
				}
				return hashCode;
			}
		}
	}
}