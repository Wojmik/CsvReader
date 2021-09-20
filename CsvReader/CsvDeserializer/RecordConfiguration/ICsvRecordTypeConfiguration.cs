using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader
{
	/// <summary>
	/// Configures record deserialization
	/// </summary>
	/// <typeparam name="TRecord">Type of record</typeparam>
	public interface ICsvRecordTypeConfiguration<TRecord>
	{
		/// <summary>
		/// Configures record deserialization
		/// </summary>
		/// <param name="recordConfiguration">Record configuration builder</param>
		void Configure(RecordConfiguration<TRecord> recordConfiguration);
	}
}