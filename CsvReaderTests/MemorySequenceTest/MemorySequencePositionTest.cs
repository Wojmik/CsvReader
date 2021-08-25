using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReaderTests.MemorySequenceTest
{
	[TestClass]
	public class MemorySequencePositionTest
	{
		static MemorySequenceSegment<char> Segment1;

		static MemorySequenceSegment<char> Segment2;

		[ClassInitialize]
		public static void PrepareData(TestContext testContext)
		{
			const string seg1Data = "Zażółć gęślą jaźń";
			const string seg2Data = "ZAŻÓŁĆ GĘŚLĄ JAŹŃ";

			Segment1 = new MemorySequenceSegment<char>(null, 128);
			seg1Data.AsSpan().CopyTo(Segment1.Array);
			Segment1.Count = seg1Data.Length;

			Segment2 = new MemorySequenceSegment<char>(null, 128);
			seg2Data.AsSpan().CopyTo(Segment2.Array);
			Segment2.Count=seg2Data.Length;
		}

		[ClassCleanup]
		public static void CleanUp()
		{
			Segment1.Dispose();
			Segment2.Dispose();
		}

		[TestMethod]
		public void EqualTest()
		{
			var pos1 = new MemorySequencePosition<char>(Segment1, 8);
			var pos2 = new MemorySequencePosition<char>(Segment1, 8);

			var pos1HashCode = pos1.GetHashCode();
			var pos2HashCode = pos2.GetHashCode();
			Assert.AreEqual(pos1HashCode, pos2HashCode, "Hash codes should be the same but are not. Pos1 hash code: {0}, pos2 hash code: {1}", pos1HashCode, pos2HashCode);
			Assert.IsTrue(pos1.Equals(pos2), "Pos1 should be equal to pos2 and is not");
		}

		[TestMethod]
		public void NotEqualDiferentSegmentsTest()
		{
			var pos1 = new MemorySequencePosition<char>(Segment1, 8);
			var pos2 = new MemorySequencePosition<char>(Segment2, 8);

			Assert.IsFalse(pos1.Equals(pos2), "Pos1 should not be equal to pos2 and is not");
		}

		[TestMethod]
		public void NotEqualDiferentPositionsTest()
		{
			var pos1 = new MemorySequencePosition<char>(Segment1, 1);
			var pos2 = new MemorySequencePosition<char>(Segment1, 14);

			Assert.IsFalse(pos1.Equals(pos2), "Pos1 should not be equal to pos2 and is not");
		}
	}
}