using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.TestItem
{
	internal class TestItemEqualityComparer : IEqualityComparer<TestItem>
	{
#if NETCOREAPP3_0_OR_GREATER
		public bool Equals(TestItem? x, TestItem? y)
#else
		public bool Equals(TestItem x, TestItem y)
#endif
		{
			return
				(x == null && y == null) ||
				(x != null && y != null
				&& x.Id == y.Id
				&& x.Text1 == y.Text1
				&& x.Text2 == y.Text2
				&& x.DayOfWeek == y.DayOfWeek
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
			if (obj != null)
			{
				hashCode = hashCode * -1521134295 + obj.Id.GetHashCode();
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Text1);
#pragma warning disable CS8604
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Text2);
#pragma warning restore CS8604
				hashCode = hashCode * -1521134295 + obj.DayOfWeek.GetHashCode();
				hashCode = hashCode * -1521134295 + obj.DayOfWeekNullable.GetHashCode();
			}
			return hashCode;
		}
	}
}