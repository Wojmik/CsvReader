using System;
using System.Collections.Generic;
using System.Text;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader
{
	readonly struct ReadCharResult
	{
		public readonly MemorySequencePosition<char> FoundPosition;

		public readonly char Character;

		public readonly bool EndOfStream;

		public ReadCharResult(in MemorySequencePosition<char> foundPosition, char character, bool endOfStream)
		{
			this.FoundPosition=foundPosition;
			this.Character=character;
			this.EndOfStream=endOfStream;
		}
	}
}