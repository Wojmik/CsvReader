using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReader.Exceptions;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.ParsingErrorTest
{
	[TestClass]
	public class CsvDeserializerParsingErrorByColumnNameTest
	{
		[TestMethod]
		public async Task ParsingErrorTest()
		{
			const string SampleCsvContent = @"Id,Text1,Text2,DayOfWeek,DayOfWeekNullable
7,Abcd,Efgh,Saturday,Friday
8,Kiba,Loba,BrokenDayOfWeek,Monday
";

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader))
			{
				var deserializationException = await Assert.ThrowsExceptionAsync<DeserializationException>(() => deserializer.ReadAllToListAsync());
				Assert.AreEqual(2, deserializationException.RowIndex);
				Assert.AreEqual(3, deserializationException.ColumnIndex);
				Assert.AreEqual("DayOfWeek", deserializationException.ColumnName);
			}
		}

		[TestMethod]
		public async Task ParsingErrorDoublieBindingsTest()
		{
			const string SampleCsvContent = @"Id,Text1,DayOfWeek,DayOfWeekNullable
7,Abcd,Saturday,Friday
8,Kiba,BrokenDayOfWeek,Monday
";

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, recordConfiguration: new RecordConfiguration_AllColumns_Required_DoubleBindings()))
			{
				var deserializationException = await Assert.ThrowsExceptionAsync<DeserializationException>(() => deserializer.ReadAllToListAsync());
				Assert.AreEqual(2, deserializationException.RowIndex);
				Assert.AreEqual(2, deserializationException.ColumnIndex);
				Assert.AreEqual("DayOfWeek", deserializationException.ColumnName);
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
	}
}