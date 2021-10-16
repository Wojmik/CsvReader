using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WojciechMikołajewicz.CsvReaderTests.TestDevices
{
	class WritableTextReader : TextReader
	{
		private string Data = string.Empty;

		public int Position { get; set; }

		public void WriteAllText(string text)
		{
			Data = text;
		}

		public override async Task<int> ReadAsync(char[] buffer, int index, int count)
		{
			return await ReadAsync(new Memory<char>(buffer, index, count))
				.ConfigureAwait(false);
		}

		public
#if NETCOREAPP2_1_OR_GREATER
			override
#endif
			ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
		{
			int toWrite = Math.Min(buffer.Length, Data.Length - Position);

			if(toWrite<=0)
				return new ValueTask<int>(0);

			Data.AsMemory(Position, toWrite).CopyTo(buffer);
			Position += toWrite;

			return new ValueTask<int>(toWrite);
		}

		public override async Task<int> ReadBlockAsync(char[] buffer, int index, int count)
		{
			return await ReadBlockAsync(new Memory<char>(buffer, index, count))
				.ConfigureAwait(false);
		}

		public
#if NETCOREAPP2_1_OR_GREATER
			override
#endif
			async ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
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