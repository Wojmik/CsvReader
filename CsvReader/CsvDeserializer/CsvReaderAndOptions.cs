using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer
{
	readonly struct CsvReaderAndOptions
	{
		public readonly CsvReader CsvReader;

		public readonly ICsvDeserializerOptions Options;

		public CsvReaderAndOptions(CsvReader csvReader, ICsvDeserializerOptions options)
		{
			CsvReader = csvReader;
			Options = options;
		}

		public void Deconstruct(out CsvReader csvReader, out ICsvDeserializerOptions options)
		{
			csvReader = CsvReader;
			options = Options;
		}
	}
}