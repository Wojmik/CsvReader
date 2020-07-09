using System;
using System.Numerics;

namespace Tester
{
	class Program
	{
		static void Main(string[] args)
		{
			ushort a = 'Ł';
			var vector1 = new Vector<ushort>('Ł');
			var vector2 = new Vector<ushort>('Ł');

			var b = Vector.EqualsAny(vector1, vector2);


			Console.WriteLine("Hello World!");
		}
	}
}