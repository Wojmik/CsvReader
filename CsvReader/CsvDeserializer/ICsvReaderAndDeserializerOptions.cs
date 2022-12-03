namespace WojciechMikołajewicz.CsvReader
{
	/// <summary>
	/// <see cref="CsvReader"/> and <see cref="CsvDeserializer{TRecord}"/> options
	/// </summary>
	public interface ICsvReaderAndDeserializerOptions : ICsvReaderOptions, ICsvDeserializerOptions
	{ }
}