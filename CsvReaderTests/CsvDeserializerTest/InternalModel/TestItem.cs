using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReaderTests.CsvDeserializerTest.InternalModel
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
				&& x.Text2==y.Text2);
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
			}
			return hashCode;
		}
	}
}