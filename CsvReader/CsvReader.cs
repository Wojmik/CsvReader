using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.MemorySequence;

namespace WojciechMikołajewicz.CsvReader
{
	public class CsvReader : IDisposable
	{
		private TextReader TextReader { get; }

		public char Delimiter { get; }
		public char EscapeChar { get; }

		private readonly System.Numerics.Vector<ushort> SearchVector;

		private MemorySequence<char> CharMemorySequence;

		private MemorySequenceSegment<char> CurrentlyLoadingSegment;

		ValueTask<int> LoadingTask;

		private int BufferSizeInChars { get; }

		public long Position { get => this.CharMemorySequence.CurrentPosition.AbsolutePosition; }

		public CsvReader(TextReader textReader)
		{
			if(textReader==null)
				throw new ArgumentNullException(nameof(textReader));

			this.TextReader=textReader;

			this.BufferSizeInChars=4096;

			//Create data for SearchVector
			ushort[] searchVactorData = new ushort[System.Numerics.Vector<ushort>.Count];
			searchVactorData[0]=Delimiter;
			searchVactorData[1]=EscapeChar;
			searchVactorData[2]='\r';
			searchVactorData[3]='\n';
			for(int i=4; i<searchVactorData.Length; i++)
				searchVactorData[i]=Delimiter;

			this.SearchVector=new System.Numerics.Vector<ushort>(searchVactorData);

			//Add first segment to CharMemorySequence
			this.CharMemorySequence.AddNewSegment(minimumLength: this.BufferSizeInChars);
		}

		public async ValueTask<MemorySequenceNode> ReadNextMemorySequenceNodeAsync(CancellationToken cancellationToken)
		{
			var foud=await FindKeyChar(checkCharMethod: IsKeyChar, cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			//Is it EndOfStream
			if(foud.EndOfStream)
				return new MemorySequenceNode(startPosition: this.CharMemorySequence.CurrentPosition, endPosition: foud.FoundPosition, nodeType: NodeType.EndOfStream, escaped: false);

			//It is not EndOfStream
			if(foud.Character==this.EscapeChar)

		}

		private async ValueTask<FindKeyCharResult> FindKeyChar(CheckCharDelegate checkCharMethod, CancellationToken cancellationToken)
		{
			var readingSegment = this.CharMemorySequence.CurrentPosition.SequenceSegment;
			var readingPositionInSegment = this.CharMemorySequence.CurrentPosition.PositionInSegment;
			char character;
			bool endOfStream = false;

			do
			{
				//Move one character forward
				readingPositionInSegment++;

				//If we are outside of a segment, start changing segment procedure
				if(readingSegment.Memory.Length<=readingPositionInSegment)
				{
					var nextReadingSegment=readingSegment.Next;

					//Is it very first load
					if(this.CurrentlyLoadingSegment==null)
					{
						//Start load data to first segment
						StartLoadSequenceSegment(charMemorySequenceSegment: this.CharMemorySequence.CurrentPosition.SequenceSegment, cancellationToken: cancellationToken);
						//Rollback segment switch
						nextReadingSegment=readingSegment;
					}

					//If we are on loading segment then wait for loading complete and start next loading
					if(object.ReferenceEquals(this.CurrentlyLoadingSegment, nextReadingSegment))
					{
						this.CurrentlyLoadingSegment.Count = await LoadingTask
							.ConfigureAwait(false);

						//Check EndOfStream
						if(this.CurrentlyLoadingSegment.Count<=0)
						{
							endOfStream=true;
							readingPositionInSegment--;
							character='\0';
							break;
						}

						//Start next loading task
						if(this.CurrentlyLoadingSegment.Next==null)
							this.CharMemorySequence.AddNewSegment(minimumLength: this.BufferSizeInChars);
						StartLoadSequenceSegment(charMemorySequenceSegment: this.CurrentlyLoadingSegment.Next, cancellationToken: cancellationToken);
					}

					//Switch to next segment
					readingSegment=nextReadingSegment;
					readingPositionInSegment=0;
				}
			}
			while(!checkCharMethod(character: character=readingSegment.Array[readingPositionInSegment]));

			return new FindKeyCharResult(foundPosition: new MemorySequencePosition<char>(sequenceSegment: readingSegment, positionInSegment: readingPositionInSegment), character: character, endOfStream: endOfStream);
		}

		private bool IsKeyChar(char character)
		{
			return System.Numerics.Vector.EqualsAny(this.SearchVector, new System.Numerics.Vector<ushort>(character));
		}

		private bool IsEscapeChar(char character)
		{
			return character==this.EscapeChar;
		}

		private void StartLoadSequenceSegment(MemorySequenceSegment<char> charMemorySequenceSegment, CancellationToken cancellationToken)
		{
			this.CurrentlyLoadingSegment=charMemorySequenceSegment;
			LoadingTask=this.TextReader.ReadBlockAsync(charMemorySequenceSegment.Array, cancellationToken);
		}

		public void Dispose()
		{
		}
	}
}