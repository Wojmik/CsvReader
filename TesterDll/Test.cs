using System;

namespace TesterDll
{
	public class Test
	{
		public static string FromSpan()
		{
			var test = new char[] { 'A', 'b', 'c' };

			return new Memory<char>(test, 0, test.Length).ToString();
		}
	}
}