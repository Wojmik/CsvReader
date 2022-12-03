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
	public class CsvDeserializerOptionalColumnByIndexTest
	{
		[TestMethod]
		public async Task RequiredColumnMissingTest()
		{
			const string SampleCsvContent = @"7,Abcd,Efgh,Saturday
";

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
			{
				options.HasHeaderRow = false;
			}, new RecordConfiguration_AllColumns_Required()))
			{
				var deserializationException = await Assert.ThrowsExceptionAsync<DeserializationException>(() => deserializer.ReadAllToListAsync());
				Assert.AreEqual(0, deserializationException.RowIndex);
				Assert.AreEqual(4, deserializationException.ColumnIndex);
				Assert.IsNull(deserializationException.ColumnName);

				Assert.IsNotNull(deserializationException.InnerException);
				Assert.IsInstanceOfType(deserializationException.InnerException, typeof(MissingBindingsException));
				var missingBindingException = (MissingBindingsException)deserializationException.InnerException;
				Assert.IsNotNull(missingBindingException.MissingColumns);
				Assert.IsTrue(Enumerable.SequenceEqual(new MissingBindingsException.ColumnInfo[] { new MissingBindingsException.ColumnInfo(null, new Nullable<int>(4)), }, missingBindingException.MissingColumns));
				Assert.AreEqual(4, missingBindingException.ColumnsCount);
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
		public async Task RequiredColumnMissingDoublieBindingsTest()
		{
			const string SampleCsvContent = @"5,Ada,Saturday
7,Abcd
";

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
			{
				options.HasHeaderRow = false;
				options.CheckColumnsCountConsistency = false;
			}, new RecordConfiguration_AllColumns_Required_DoubleBindings()))
			{
				var deserializationException = await Assert.ThrowsExceptionAsync<DeserializationException>(() => deserializer.ReadAllToListAsync());
				Assert.AreEqual(1, deserializationException.RowIndex);
				Assert.AreEqual(2, deserializationException.ColumnIndex);
				Assert.IsNull(deserializationException.ColumnName);

				Assert.IsNotNull(deserializationException.InnerException);
				Assert.IsInstanceOfType(deserializationException.InnerException, typeof(MissingBindingsException));
				var missingBindingException = (MissingBindingsException)deserializationException.InnerException;
				Assert.IsNotNull(missingBindingException.MissingColumns);
				Assert.IsTrue(Enumerable.SequenceEqual(new MissingBindingsException.ColumnInfo[] { new MissingBindingsException.ColumnInfo(null, new Nullable<int>(2)), new MissingBindingsException.ColumnInfo(null, new Nullable<int>(2)), }, missingBindingException.MissingColumns));
				Assert.AreEqual(2, missingBindingException.ColumnsCount);
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
				recordConfiguration.PropertyEnum(r => r.DayOfWeekNullable).BindToColumn(2);
			}
		}

		[TestMethod]
		public async Task OptionalColumnPresentTest()
		{
			const string SampleCsvContent = @"7,Abcd,Efgh,Saturday,Friday
";
			var expected = new TestItem.TestItem[]
			{
				new TestItem.TestItem() { Id = 7, Text1 = "Abcd", Text2 = "Efgh", DayOfWeek = DayOfWeek.Saturday, DayOfWeekNullable = DayOfWeek.Friday, },
			};

			List<TestItem.TestItem> actual;

			var recordConf = new RecordConfiguration_DayOfWeekNullable_Optional();

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
			{
				options.HasHeaderRow = false;
			}, recordConf))
			{
				actual = await deserializer.ReadAllToListAsync();
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}

		[TestMethod]
		public async Task OptionalColumnNotPresentTest()
		{
			const string SampleCsvContent = @"7,Abcd,Efgh,Saturday
";
			var expected = new TestItem.TestItem[]
			{
				new TestItem.TestItem() { Id = 7, Text1 = "Abcd", Text2 = "Efgh", DayOfWeek = DayOfWeek.Saturday, DayOfWeekNullable = null, },
			};

			List<TestItem.TestItem> actual;

			var recordConf = new RecordConfiguration_DayOfWeekNullable_Optional();

			using (var textReader = new StringReader(SampleCsvContent))
			using (var deserializer = new CsvDeserializer<TestItem.TestItem>(textReader, options =>
			{
				options.HasHeaderRow = false;
			}, recordConf))
			{
				actual = await deserializer.ReadAllToListAsync();
			}

			Assert.IsTrue(Enumerable.SequenceEqual(expected, actual, new TestItemEqualityComparer()));
		}

		class RecordConfiguration_DayOfWeekNullable_Optional : ICsvRecordTypeConfiguration<TestItem.TestItem>
		{
			public void Configure(RecordConfiguration<TestItem.TestItem> recordConfiguration)
			{
				recordConfiguration.Property(r => r.Id).BindToColumn(0);
				recordConfiguration.Property(r => r.Text1).BindToColumn(1);
				recordConfiguration.Property(r => r.Text2).BindToColumn(2);
				recordConfiguration.PropertyEnum(r => r.DayOfWeek).BindToColumn(3);
				recordConfiguration.PropertyEnum(r => r.DayOfWeekNullable).BindToColumn(4).IsOptional();
			}
		}
	}
}