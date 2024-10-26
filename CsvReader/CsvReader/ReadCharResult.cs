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
			FoundPosition = foundPosition;
			Character = character;
			EndOfStream = endOfStream;
		}
	}
}