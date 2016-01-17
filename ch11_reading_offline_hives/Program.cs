using System;
using ntregsharp;

namespace ch11_reading_offline_hives
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			RegistryHive hive = new RegistryHive (args [0]);

			if (hive.WasExported)
				Console.Write ("This hive was exported. ");
			
			Console.WriteLine("The rootkey's name is " + hive.RootKey + ".");
		}
	}
}
