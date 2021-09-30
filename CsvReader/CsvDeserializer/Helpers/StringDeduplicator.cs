using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.Helpers
{
	public readonly struct StringDeduplicator
	{
		public static StringDeduplicator Create()
		{
#if NETSTANDARD2_1_OR_GREATER
			return new StringDeduplicator(new HashSet<string>(StringComparer.Ordinal));
#else
			return new StringDeduplicator(new Dictionary<string, string>(StringComparer.Ordinal));
#endif
		}

#if NETSTANDARD2_1_OR_GREATER
		private readonly HashSet<string> StringSet;

		private StringDeduplicator(HashSet<string> stringSet)
		{
			StringSet = stringSet;
		}
#else
		private readonly Dictionary<string, string> StringSet;

		private StringDeduplicator(Dictionary<string, string> stringSet)
		{
			StringSet = stringSet;
		}
#endif

		public string Deduplicate(string value)
		{
			if(StringSet.TryGetValue(value, out string storedValue))
				return storedValue;

#if NETSTANDARD2_1_OR_GREATER
			StringSet.Add(value);
#else
			StringSet.Add(value, value);
#endif

			return value;
		}
	}
}