using System;
using ntregsharp;

namespace ch11_reading_offline_hives
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			RegistryHive hive = new RegistryHive (args [0]);
			
			Console.WriteLine("The rootkey's name is " + hive.RootKey.Name);
		}
	}
}
