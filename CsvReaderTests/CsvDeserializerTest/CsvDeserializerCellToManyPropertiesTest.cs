using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.DeserializerConfiguration;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest
{
	[TestClass]
	public class CsvDeserializerCellToManyPropertiesTest
	{
		static IEnumerable<object[]> GetSampleData()
		{
			yield return new object[]
			{
				@"Id
A5d2fb0283c36a01
452b82C32251F82d
",
				new TestItem[]
				{
					new TestItem(){ Id=-6497855324123076095, IdText="A5d2fb0283c36a01", FromBase64=new byte[] { 3, 151, 118, 125, 189, 54, 243, 119, 55, 233, 173, 53, }, FromHex=new byte[] { 165, 210, 251, 2, 131, 195, 106, 1, }, },
					new TestItem(){ Id=4984221187221616685, IdText="452b82C32251F82d", FromBase64=new byte[] { 227, 157, 155, 243, 96, 183, 219, 110, 117, 23, 205, 157, }, FromHex=new byte[] { 69, 43, 130, 195, 34, 81, 248, 45, }, },
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
				recordConfiguration
					.Property(rec => rec.Id)
					.BindToColumn(nameof(TestItem.Id))
					.ConfigureDeserializer(deserializer =>
					{
						deserializer.SetNumberStyles(NumberStyles.HexNumber);
					});

				recordConfiguration
					.Property(rec => rec.IdText)
					.BindToColumn(nameof(TestItem.Id))
					.ConfigureDeserializer(deserializer =>
					{
					});

				recordConfiguration
					.Property(rec => rec.FromBase64)
					.BindToColumn(nameof(TestItem.Id))
					.ConfigureDeserializer(deserializer =>
					{
						deserializer.SetByteArrayEncoding(ByteArrayEncoding.Base64);
					});

				recordConfiguration
					.Property(rec => rec.FromHex)
					.BindToColumn(nameof(TestItem.Id))
					.ConfigureDeserializer(deserializer =>
					{
						deserializer.SetByteArrayEncoding(ByteArrayEncoding.Hex);
					});
			}
		}

		public class TestItem
		{
			public long Id { get; set; }

#pragma warning disable CS8618
			public string IdText { get; set; }

			public byte[] FromBase64 { get; set; }

			public byte[] FromHex { get; set; }
#pragma warning restore CS8618
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
					&& x.IdText==y.IdText
					&& Enumerable.SequenceEqual(x.FromBase64, y.FromBase64)
					&& Enumerable.SequenceEqual(x.FromHex, y.FromHex)
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
					hashCode=hashCode*-1521134295+EqualityComparer<string>.Default.GetHashCode(obj.IdText);
					if(obj.FromBase64!=null)
						foreach(var bt in obj.FromBase64)
							hashCode=hashCode*-1521134295+bt.GetHashCode();
					if(obj.FromHex!=null)
						foreach(var bt in obj.FromHex)
							hashCode=hashCode*-1521134295+bt.GetHashCode();
				}
				return hashCode;
			}
		}
	}
}