using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReaderTests.TestDevices
{
	class RepeatedTextReader : TextReader
	{
		public string RepeatedText { get; }

		public int MaxReadSize { get; set; } = int.MaxValue;

		public long Position { get; set; }

		public long Length { get; set; } = long.MaxValue;

		public RepeatedTextReader(string repeatedText)
		{
			if(repeatedText==null)
				throw new ArgumentNullException(nameof(repeatedText));
			if(repeatedText.Length<=0)
				throw new ArgumentException($"{nameof(repeatedText)} cannot be empty", nameof(repeatedText));

			this.RepeatedText = repeatedText;
		}

		public override async Task<int> ReadAsync(char[] buffer, int index, int count)
		{
			return await ReadAsync(new Memory<char>(buffer, index, count))
				.ConfigureAwait(false);
		}

		public override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
		{
			int toWrite = Math.Min(buffer.Length, (int)Math.Min(MaxReadSize, Length - Position)), written = 0, position, chunkSize;

			while(written<toWrite)
			{
				position = (int)(Position % RepeatedText.Length);
				chunkSize = Math.Min(RepeatedText.Length-position, toWrite-written);
				RepeatedText.AsSpan(position, chunkSize).CopyTo(buffer.Span.Slice(written));
				Position += chunkSize;
				written += chunkSize;
			}

			return ValueTask.FromResult(toWrite);
		}

		public override async Task<int> ReadBlockAsync(char[] buffer, int index, int count)
		{
			return await ReadBlockAsync(new Memory<char>(buffer, index, count))
				.ConfigureAwait(false);
		}

		public override async ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
		{
			int written = 0, chunkSize;

			while(written<buffer.Length && 0<(chunkSize = await ReadAsync(buffer.Slice(written), cancellationToken)))
			{
				written += chunkSize;
			}

			return written;
		}
	}
}