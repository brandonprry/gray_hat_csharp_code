using System;

namespace ch1_hello_world
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string hello = "Hello World!";
			DateTime now = DateTime.Now;
			Console.Write(hello);
			Console.WriteLine (" The date is " + now.ToLongDateString());
		}
	}
}
