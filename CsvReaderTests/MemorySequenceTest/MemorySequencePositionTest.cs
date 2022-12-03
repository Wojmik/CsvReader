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
#pragma warning disable CS8618
		static MemorySequenceSegment<char> _segment1;

		static MemorySequenceSegment<char> _segment2;
#pragma warning restore CS8618

		[ClassInitialize]
		public static void PrepareData(TestContext testContext)
		{
			const string Seg1Data = "Zażółć gęślą jaźń";
			const string Seg2Data = "ZAŻÓŁĆ GĘŚLĄ JAŹŃ";

			_segment1 = new MemorySequenceSegment<char>(null, 128);
			Seg1Data.AsSpan().CopyTo(_segment1.Array);
			_segment1.Count = Seg1Data.Length;

			_segment2 = new MemorySequenceSegment<char>(null, 128);
			Seg2Data.AsSpan().CopyTo(_segment2.Array);
			_segment2.Count=Seg2Data.Length;
		}

		[ClassCleanup]
		public static void CleanUp()
		{
			_segment1.Dispose();
			_segment2.Dispose();
		}

		[TestMethod]
		public void EqualTest()
		{
			var pos1 = new MemorySequencePosition<char>(_segment1, 8);
			var pos2 = new MemorySequencePosition<char>(_segment1, 8);

			var pos1HashCode = pos1.GetHashCode();
			var pos2HashCode = pos2.GetHashCode();
			Assert.AreEqual(pos1HashCode, pos2HashCode, "Hash codes should be the same but are not. Pos1 hash code: {0}, pos2 hash code: {1}", pos1HashCode, pos2HashCode);
			Assert.IsTrue(pos1.Equals(pos2), "Pos1 should be equal to pos2 and is not");
		}

		[TestMethod]
		public void NotEqualDiferentSegmentsTest()
		{
			var pos1 = new MemorySequencePosition<char>(_segment1, 8);
			var pos2 = new MemorySequencePosition<char>(_segment2, 8);

			Assert.IsFalse(pos1.Equals(pos2), "Pos1 should not be equal to pos2 and is not");
		}

		[TestMethod]
		public void NotEqualDiferentPositionsTest()
		{
			var pos1 = new MemorySequencePosition<char>(_segment1, 1);
			var pos2 = new MemorySequencePosition<char>(_segment1, 14);

			Assert.IsFalse(pos1.Equals(pos2), "Pos1 should not be equal to pos2 and is not");
		}
	}
}