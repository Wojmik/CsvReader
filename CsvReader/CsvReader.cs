using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CharMemorySequence;

namespace WojciechMikołajewicz.CsvReader
{
	public class CsvReader : IDisposable
	{
		private TextReader TextReader { get; }

		public char Delimiter { get; }
		public char EscapeChar { get; }

		private readonly System.Numerics.Vector<ushort> SearchVector;

		private CharMemorySequence.CharMemorySequence CharMemorySequence;

		private CharMemorySequenceSegment CurrentlyLoadingSegment;

		ValueTask<int> LoadingTask;

		private int BufferSizeInChars { get; }

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

			//Create CharMemorySequence and first segment
			this.CharMemorySequence.First=new CharMemorySequenceSegment(previous: null, minimumLength: this.BufferSizeInChars);
			this.CharMemorySequence.Last=this.CharMemorySequence.First;
		}

		public async ValueTask<CharArrayNode> ReadNextCharArrayNodeAsync()
		{
			
		}

		private async ValueTask<long> FindKeyChar(CancellationToken cancellationToken)
		{
			//If there isn't anything to read, flip first to last
			while(this.CharMemorySequence.First.Count<=0)
			{
				//Should I run loading task (very first load)
				if(this.CurrentlyLoadingSegment==null)
					StartLoadSequenceSegment(charMemorySequenceSegment: this.CharMemorySequence.First, cancellationToken: cancellationToken);

				//If we are on loading segment then wait for loading complete and start next loading
				if(object.ReferenceEquals(this.CurrentlyLoadingSegment, this.CharMemorySequence.First))
				{
					this.CurrentlyLoadingSegment.Count = await LoadingTask.ConfigureAwait(false);
					
					//Check EndOfStream
					if(this.CurrentlyLoadingSegment.Count<=0)
						return -1;

					//Start next loading task
					if(this.CurrentlyLoadingSegment.Next==null)
						this.CharMemorySequence.AddNewSegment(minimumLength: this.BufferSizeInChars);
					StartLoadSequenceSegment(charMemorySequenceSegment: this.CurrentlyLoadingSegment.Next, cancellationToken: cancellationToken);
					break;
				}

				//Flip first to last
				this.CharMemorySequence.FlipFirstToLast();
			}

			


		}

		private void StartLoadSequenceSegment(CharMemorySequenceSegment charMemorySequenceSegment, CancellationToken cancellationToken)
		{
			this.CurrentlyLoadingSegment=charMemorySequenceSegment;
			LoadingTask=this.TextReader.ReadBlockAsync(charMemorySequenceSegment.Segment, cancellationToken);
		}

		public void Dispose()
		{
		}
	}

	public class DataChunk<T> : ReadOnlySequenceSegment<T>
	{
		public DataChunk()
		{
			System.Numerics.Vector<ushort> vector;
			System.Numerics.Vector.

			new System.IO.StreamReader("")
		}
	}
}