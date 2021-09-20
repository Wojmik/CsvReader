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
		const string CsvString = @"Id,Text
7,Abcd";

		[TestMethod]
		public async Task DeserializeTestAsync()
		{
			var actual = new List<TestItem>();

			var recordConf = new RecordConfigurator();

			using(var textReader = new StringReader(CsvString))
			using(var deserializer = new CsvDeserializer<TestItem>(textReader, recordConfiguration: recordConf))
			{
#if NETCOREAPP3_0_OR_GREATER
				await foreach(var item in deserializer.ReadAsync())
					actual.Add(item);
#endif
			}
		}

		class RecordConfigurator : ICsvRecordTypeConfiguration<TestItem>
		{
			public void Configure(RecordConfiguration<TestItem> recordConfiguration)
			{
				recordConfiguration
					.Property(rec => rec.Text)
					.BindToColumn(nameof(TestItem.Text))
					.ConfigureDeserializer(deserializer =>
					{
					});

				recordConfiguration
					.Property(rec => rec.Id)
					.BindToColumn(nameof(TestItem.Id))
					.ConfigureDeserializer(deserializer =>
					{
					});
			}
		}
	}
}