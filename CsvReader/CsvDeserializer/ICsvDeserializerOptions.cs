﻿using System;
using System.Collections.Generic;

namespace WojciechMikołajewicz.CsvReader
{
	/// <summary>
	/// <see cref="CsvDeserializer{TRecord}"/> options
	/// </summary>
	public interface ICsvDeserializerOptions
	{
		/// <summary>
		/// Does csv contain header row. Default is true
		/// </summary>
		bool HasHeaderRow { get; set; }

		/// <summary>
		/// Comparer of column names in header row. Default is <see cref="StringComparer.OrdinalIgnoreCase"/>
		/// </summary>
		IEqualityComparer<string> HeaderRowColumnNamesComparer { get; set; }

		/// <summary>
		/// Culture used for parsing values from csv file. Default is <see cref="CultureInfo.InvariantCulture"/>
		/// </summary>
		IFormatProvider DeserializationCulture { get; set; }

		/// <summary>
		/// If empty strings should be converted to nulls. Default is true
		/// </summary>
		bool EmptyAsNull { get; set; }

		/// <summary>
		/// If strings with the same value should be deduplicated. Default is true
		/// </summary>
		/// <remarks>
		/// Set it to true if you want to store all records in memory at once - it will reduce memory consumption.
		/// Set it to false if you want to read, process and forget records one by one.
		/// </remarks>
		bool DeduplicateStrings { get; set; }

		/// <summary>
		/// Check if columns count is equal in every row. Default is true
		/// </summary>
		bool CheckColumnsCountConsistency { get; set; }
	}
}