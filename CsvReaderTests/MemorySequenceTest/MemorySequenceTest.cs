using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReaderTests.MemorySequenceTest
{
	[TestClass]
	public class MemorySequenceTest
	{
		[TestMethod]
		public void MoveForwardTest()
		{
			using var memSeq = new MemorySequence<char>();

			memSeq.AddNewSegment(minimumLength: 32);
			var seg0 = memSeq.CurrentPosition.InternalSequenceSegment;
			WriteSegment(seg0, seg0.Array.Length, "Seg 0 seq 0 ");

			memSeq.AddNewSegment(minimumLength: 32);
			var seg1 = seg0.NextInternal;
			WriteSegment(seg1, seg1.Array.Length, "Seg 1 seq 1 ");

			Assert.AreEqual(new MemorySequencePosition<char>(seg0, 0), memSeq.CurrentPosition);
			Assert.AreEqual(0, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg1, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);

			memSeq.MoveForward(new MemorySequencePosition<char>(seg0, 10), 0);
			Assert.AreEqual(new MemorySequencePosition<char>(seg0, 10), memSeq.CurrentPosition);
			Assert.AreEqual(10, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg1, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);

			memSeq.MoveForward(memSeq.CurrentPosition, 5);
			Assert.AreEqual(new MemorySequencePosition<char>(seg0, 15), memSeq.CurrentPosition);
			Assert.AreEqual(15, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg1, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);

			memSeq.MoveForward(new MemorySequencePosition<char>(seg0, 28), 0);
			Assert.AreEqual(new MemorySequencePosition<char>(seg0, 28), memSeq.CurrentPosition);
			Assert.AreEqual(28, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg1, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);

			memSeq.MoveForward(new MemorySequencePosition<char>(seg0, seg0.Array.Length-1), 0);
			Assert.AreEqual(new MemorySequencePosition<char>(seg0, seg0.Array.Length-1), memSeq.CurrentPosition);
			Assert.AreEqual(seg0.Array.Length-1, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg1, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);

			memSeq.MoveForward(new MemorySequencePosition<char>(seg1, 0), 0);
			Assert.AreEqual(new MemorySequencePosition<char>(seg1, 0), memSeq.CurrentPosition);
			Assert.AreEqual(seg0.Array.Length, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg0, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);
			WriteSegment(seg0, seg0.Array.Length, "Seg 0 seq 2 ");

			memSeq.MoveForward(memSeq.CurrentPosition, 30);
			Assert.AreEqual(new MemorySequencePosition<char>(seg1, 30), memSeq.CurrentPosition);
			Assert.AreEqual(seg0.Array.Length+30, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg0, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);

			memSeq.MoveForward(memSeq.CurrentPosition, seg1.Array.Length-memSeq.CurrentPosition.PositionInSegment+17);
			Assert.AreEqual(new MemorySequencePosition<char>(seg0, 17), memSeq.CurrentPosition);
			Assert.AreEqual(seg0.Array.Length+seg1.Array.Length+17, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg1, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);
		}

		[TestMethod]
		public void MoveForwardFlipTwoSegmentsTest()
		{
			using var memSeq = new MemorySequence<char>();

			memSeq.AddNewSegment(minimumLength: 32);
			var seg0 = memSeq.CurrentPosition.InternalSequenceSegment;
			WriteSegment(seg0, seg0.Array.Length, "Seg 0 seq 0 ");

			memSeq.AddNewSegment(minimumLength: 32);
			var seg1 = seg0.NextInternal;
			WriteSegment(seg1, seg1.Array.Length, "Seg 1 seq 1 ");

			memSeq.AddNewSegment(minimumLength: 32);
			var seg2 = seg1.NextInternal;
			WriteSegment(seg2, seg2.Array.Length, "Seg 2 seq 2 ");

			memSeq.MoveForward(new MemorySequencePosition<char>(seg2, 30), 0);
			Assert.AreEqual(new MemorySequencePosition<char>(seg2, 30), memSeq.CurrentPosition);
			Assert.AreEqual(seg0.Array.Length+seg1.Array.Length+30, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg0, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);
			Assert.AreSame(seg1, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal.NextInternal);
			
			WriteSegment(seg0, seg0.Array.Length, "Seg 0 seq 3 ");
			WriteSegment(seg1, seg1.Array.Length, "Seg 1 seq 4 ");

			memSeq.MoveForward(new MemorySequencePosition<char>(seg1, 5), 18);
			Assert.AreEqual(new MemorySequencePosition<char>(seg1, 23), memSeq.CurrentPosition);
			Assert.AreEqual(seg0.Array.Length+seg1.Array.Length+seg2.Array.Length+seg0.Array.Length+23, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg2, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);
			Assert.AreSame(seg0, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal.NextInternal);
		}

		[TestMethod]
		public void MoveForwardToFar1Test()
		{
			using var memSeq = new MemorySequence<char>();

			memSeq.AddNewSegment(minimumLength: 32);
			var seg0 = memSeq.CurrentPosition.InternalSequenceSegment;
			WriteSegment(seg0, seg0.Array.Length, "Seg 0 seq 0 ");

			memSeq.AddNewSegment(minimumLength: 32);
			var seg1 = seg0.NextInternal;
			WriteSegment(seg1, seg1.Array.Length, "Seg 1 seq 1 ");

			memSeq.MoveForward(new MemorySequencePosition<char>(seg1, 30), 0);
			Assert.AreEqual(new MemorySequencePosition<char>(seg1, 30), memSeq.CurrentPosition);
			Assert.AreEqual(seg0.Array.Length+30, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg0, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);

			memSeq.MoveForward(memSeq.CurrentPosition, seg1.Array.Length-memSeq.CurrentPosition.PositionInSegment);
			Assert.AreEqual(new MemorySequencePosition<char>(seg0, 0), memSeq.CurrentPosition);
			Assert.AreEqual(seg0.Array.Length+seg1.Array.Length, memSeq.CurrentPosition.AbsolutePosition);
			Assert.AreSame(seg1, memSeq.CurrentPosition.InternalSequenceSegment.NextInternal);

			Assert.ThrowsException<InvalidOperationException>(() => memSeq.MoveForward(memSeq.CurrentPosition, 1));
		}

		[TestMethod]
		public void MoveForwardToFar2Test()
		{
			using var memSeq = new MemorySequence<char>();

			memSeq.AddNewSegment(minimumLength: 32);
			var seg0 = memSeq.CurrentPosition.InternalSequenceSegment;
			WriteSegment(seg0, seg0.Array.Length-1, "Seg 0 seq 0 ");

			memSeq.AddNewSegment(minimumLength: 32);
			var seg1 = seg0.NextInternal;

			Assert.ThrowsException<InvalidOperationException>(() => memSeq.MoveForward(new MemorySequencePosition<char>(seg1, 0), 0));
		}

		[TestMethod]
		public void MoveForwardToFar3Test()
		{
			using var memSeq = new MemorySequence<char>();

			memSeq.AddNewSegment(minimumLength: 32);
			var seg0 = memSeq.CurrentPosition.InternalSequenceSegment;
			WriteSegment(seg0, seg0.Array.Length-1, "Seg 0 seq 0 ");

			memSeq.AddNewSegment(minimumLength: 32);
			var seg1 = seg0.NextInternal;

			Assert.ThrowsException<InvalidOperationException>(() => memSeq.MoveForward(memSeq.CurrentPosition, seg0.Array.Length));
		}

		[TestMethod]
		public void MoveForwardToAlienSegmentTest()
		{
			using var memSeq = new MemorySequence<char>();

			memSeq.AddNewSegment(minimumLength: 32);
			var seg0 = memSeq.CurrentPosition.InternalSequenceSegment;
			WriteSegment(seg0, seg0.Array.Length, "Seg 0 seq 0 ");

			memSeq.AddNewSegment(minimumLength: 32);
			var seg1 = seg0.NextInternal;
			WriteSegment(seg1, seg1.Array.Length, "Seg 1 seq 1 ");

			using var seg2 = new MemorySequenceSegment<char>(null, 32);
			WriteSegment(seg2, seg2.Array.Length, "Seg 2 seq alien ");

			Assert.ThrowsException<ArgumentException>(() => memSeq.MoveForward(new MemorySequencePosition<char>(seg2, 0), 0));
		}

		private void WriteSegment(MemorySequenceSegment<char> segment, int toWrite, string text)
		{
			int textLength = Math.Min(text.Length, toWrite);
			text.CopyTo(0, segment.Array, 0, textLength);

			for(int i = textLength; i<toWrite; i++)
				segment.Array[i] = (char)('0'+(i%10));

			segment.Count = toWrite;
		}
	}
}