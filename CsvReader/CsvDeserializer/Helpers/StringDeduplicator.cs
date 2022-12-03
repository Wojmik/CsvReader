using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.Helpers
{
	/// <summary>
	/// Class providing string deduplication capability
	/// </summary>
	public readonly struct StringDeduplicator
	{
		/// <summary>
		/// Creates string deduplicator
		/// </summary>
		/// <returns>String deduplicator</returns>
		public static StringDeduplicator Create()
		{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
			return new StringDeduplicator(new HashSet<string>(StringComparer.Ordinal));
#else
			return new StringDeduplicator(new Dictionary<string, string>(StringComparer.Ordinal));
#endif
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
		private readonly HashSet<string> _stringSet;

		private StringDeduplicator(HashSet<string> stringSet)
		{
			_stringSet = stringSet;
		}
#else
		private readonly Dictionary<string, string> _stringSet;

		private StringDeduplicator(Dictionary<string, string> stringSet)
		{
			_stringSet = stringSet;
		}
#endif

		/// <summary>
		/// Deduplicates string
		/// </summary>
		/// <param name="value">String to deduplicate</param>
		/// <returns>String instance that is already in string deduplicator container or <paramref name="value"/> if it wasn't exist yiet in the container</returns>
		public string Deduplicate(string value)
		{
			if(_stringSet.TryGetValue(value, out string? storedValue))
				return storedValue;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
			_stringSet.Add(value);
#else
			_stringSet.Add(value, value);
#endif

			return value;
		}
	}
}