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
	public class CsvDeserializerParsingErrorTest
	{
		[TestMethod]
		public async Task ParsingErrorTest()
		{
			const string SampleCsvContent = @"7,Abcd,Efgh,Saturday,Friday
8,Kiba,Loba,BrokenDayOfWeek,Monday
";

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
			{
				options.HasHeaderRow = false;
			}, new RecordConfiguration_AllColumns_Required()))
			{
				var deserializationException = await Assert.ThrowsExceptionAsync<DeserializationException>(() => deserializer.ReadAllToListAsync());
				Assert.AreEqual(1, deserializationException.RowIndex);
				Assert.AreEqual(3, deserializationException.ColumnIndex);
				Assert.IsNull(deserializationException.ColumnName);
			}
		}

		class RecordConfiguration_AllColumns_Required : ICsvRecordTypeConfiguration<TestItem.TestItem>
		{
			public void Configure(RecordConfiguration<TestItem.TestItem> recordConfiguration)
			{
				recordConfiguration.Property(r => r.Id).BindToColumn(0);
				recordConfiguration.Property(r => r.Text1).BindToColumn(1);
				recordConfiguration.Property(r => r.Text2).BindToColumn(2);
				recordConfiguration.PropertyEnum(r => r.DayOfWeek).BindToColumn(3);
				recordConfiguration.PropertyEnum(r => r.DayOfWeekNullable).BindToColumn(4);
			}
		}

		[TestMethod]
		public async Task ParsingErrorDoublieBindingsTest()
		{
			const string SampleCsvContent = @"7,Abcd,Saturday,Friday
8,Kiba,BrokenDayOfWeek,Monday
";

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
			{
				options.HasHeaderRow = false;
			}, new RecordConfiguration_AllColumns_Required_DoubleBindings()))
			{
				var deserializationException = await Assert.ThrowsExceptionAsync<DeserializationException>(() => deserializer.ReadAllToListAsync());
				Assert.AreEqual(1, deserializationException.RowIndex);
				Assert.AreEqual(2, deserializationException.ColumnIndex);
				Assert.IsNull(deserializationException.ColumnName);
			}
		}

		class RecordConfiguration_AllColumns_Required_DoubleBindings : ICsvRecordTypeConfiguration<TestItem.TestItem>
		{
			public void Configure(RecordConfiguration<TestItem.TestItem> recordConfiguration)
			{
				recordConfiguration.Property(r => r.Id).BindToColumn(0);
				recordConfiguration.Property(r => r.Text1).BindToColumn(1);
				recordConfiguration.Property(r => r.Text2).BindToColumn(1);
				recordConfiguration.PropertyEnum(r => r.DayOfWeek).BindToColumn(2);
				recordConfiguration.PropertyEnum(r => r.DayOfWeekNullable).BindToColumn(3);
			}
		}
	}
}