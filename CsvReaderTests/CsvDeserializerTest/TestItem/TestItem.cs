using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.TestItem
{
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
}