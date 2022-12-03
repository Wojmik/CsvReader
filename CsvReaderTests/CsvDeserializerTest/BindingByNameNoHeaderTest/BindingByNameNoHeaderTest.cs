using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader;
using WojciechMikołajewicz.CsvReader.Exceptions;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.BindingByNameNoHeaderTest
{
	[TestClass]
	public class BindingByNameNoHeaderTest
	{
		[TestMethod]
		public void NoRecordConfigurationTest()
		{
			const string SampleCsvContent = @"7,Abcd,Efgh,Saturday,Friday
";

			using (var textReader = new StringReader(SampleCsvContent))
			{
				CsvDeserializer<TestItem.TestItem> deserializer = null;
				try
				{
					var exception = Assert.ThrowsException<ArgumentNullException>(() => deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
					{
						options.HasHeaderRow = false;
					}));
					Assert.AreEqual("recordConfiguration", exception.ParamName);
				}
				finally
				{
					deserializer?.Dispose();
				}
			}
		}

		[TestMethod]
		public void BindingByNameTest()
		{
			const string SampleCsvContent = @"7,Abcd,Efgh,Saturday,Friday
";

			using (var textReader = new StringReader(SampleCsvContent))
			{
				CsvDeserializer<TestItem.TestItem> deserializer = null;
				try
				{
					Assert.ThrowsException<BindingConfigurationException>(() => deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
					{
						options.HasHeaderRow = false;
					}, new RecordConfiguration_Text2_ByName()));
				}
				finally
				{
					deserializer?.Dispose();
				}
			}
		}

		class RecordConfiguration_Text2_ByName : ICsvRecordTypeConfiguration<TestItem.TestItem>
		{
			public void Configure(RecordConfiguration<TestItem.TestItem> recordConfiguration)
			{
				recordConfiguration.Property(r => r.Id).BindToColumn(0);
				recordConfiguration.Property(r => r.Text1).BindToColumn(1);
				recordConfiguration.Property(r => r.Text2).BindToColumn("Text2");
				recordConfiguration.PropertyEnum(r => r.DayOfWeek).BindToColumn(3);
				recordConfiguration.PropertyEnum(r => r.DayOfWeekNullable).BindToColumn(4).IsOptional();
			}
		}
	}
}