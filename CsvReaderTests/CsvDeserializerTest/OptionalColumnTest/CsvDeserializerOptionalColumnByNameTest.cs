using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReader.Exceptions;
using WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.TestItem;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.OptionalColumnTest
{
	[TestClass]
	public class CsvDeserializerOptionalColumnByNameTest
	{
		[TestMethod]
		public async Task RequiredColumnMissingTest()
		{
			const string SampleCsvContent = @"Id,Text2,DayOfWeek,DayOfWeekNullable
7,Efgh,Saturday,Friday
";

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader))
			{
				var missingBindingException = await Assert.ThrowsExceptionAsync<MissingBindingsException>(() => deserializer.ReadAllToListAsync());
				Assert.IsNotNull(missingBindingException.MissingColumns);
				Assert.IsTrue(Enumerable.SequenceEqual(new MissingBindingsException.ColumnInfo[] { new MissingBindingsException.ColumnInfo("Text1", null), }, missingBindingException.MissingColumns));
				Assert.AreEqual(4, missingBindingException.ColumnsCount);
			}
		}
		
		[TestMethod]
		public async Task RequiredColumnMissingDoublieBindingsTest()
		{
			const string SampleCsvContent = @"Id,Text1,DayOfWeekNullable
7,Abcd,Friday
";

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, recordConfiguration: new RecordConfiguration_AllColumns_Required_DoubleBindings()))
			{
				var missingBindingException = await Assert.ThrowsExceptionAsync<MissingBindingsException>(() => deserializer.ReadAllToListAsync());
				Assert.IsNotNull(missingBindingException.MissingColumns);
				Assert.IsTrue(Enumerable.SequenceEqual(new MissingBindingsException.ColumnInfo[] { new MissingBindingsException.ColumnInfo("DayOfWeek", null), }, missingBindingException.MissingColumns));
				Assert.AreEqual(3, missingBindingException.ColumnsCount);
			}
		}

		class RecordConfiguration_AllColumns_Required_DoubleBindings : ICsvRecordTypeConfiguration<TestItem.TestItem>
		{
			public void Configure(RecordConfiguration<TestItem.TestItem> recordConfiguration)
			{
				recordConfiguration.Property(r => r.Id).BindToColumn("Id");
				recordConfiguration.Property(r => r.Text1).BindToColumn("Text1");
				recordConfiguration.Property(r => r.Text2).BindToColumn("Text1");
				recordConfiguration.PropertyEnum(r => r.DayOfWeek).BindToColumn("DayOfWeek");
				recordConfiguration.PropertyEnum(r => r.DayOfWeekNullable).BindToColumn("DayOfWeekNullable");
			}
		}

		[TestMethod]
		public async Task OptionalColumnPresentTest()
		{
			const string SampleCsvContent = @"Id,Text1,Text2,DayOfWeek,DayOfWeekNullable
7,Abcd,Efgh,Saturday,Friday
";
			var expected = new TestItem.TestItem[]
			{
				new TestItem.TestItem() { Id = 7, Text1 = "Abcd", Text2 = "Efgh", DayOfWeek = DayOfWeek.Saturday, DayOfWeekNullable = DayOfWeek.Friday, },
			};

			List<TestItem.TestItem> actual;

			var recordConf = new RecordConfiguration_Text2_Optional();

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, recordConfiguration: recordConf))
			{
				actual = await deserializer.ReadAllToListAsync();
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}

		[TestMethod]
		public async Task OptionalColumnNotPresentTest()
		{
			const string SampleCsvContent = @"Id,Text1,DayOfWeek,DayOfWeekNullable
7,Abcd,Saturday,Friday
";
			var expected = new TestItem.TestItem[]
			{
				new TestItem.TestItem() { Id = 7, Text1 = "Abcd", Text2 = null, DayOfWeek = DayOfWeek.Saturday, DayOfWeekNullable = DayOfWeek.Friday, },
			};

			List<TestItem.TestItem> actual;

			var recordConf = new RecordConfiguration_Text2_Optional();

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, recordConfiguration: recordConf))
			{
				actual = await deserializer.ReadAllToListAsync();
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}

		class RecordConfiguration_Text2_Optional : ICsvRecordTypeConfiguration<TestItem.TestItem>
		{
			public void Configure(RecordConfiguration<TestItem.TestItem> recordConfiguration)
			{
				recordConfiguration.Property(r => r.Text2)
					.IsOptional();
			}
		}
	}
}