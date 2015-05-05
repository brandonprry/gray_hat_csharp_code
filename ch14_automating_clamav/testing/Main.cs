using System;
using clamsharp;

namespace testing
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (ClamEngine e = new ClamEngine())
			{
				foreach (string file in args)
				{
					ClamResult result = e.ScanFile(file); //pretty simple!
					
					if (result != null && result.ReturnCode == ClamReturnCode.CL_VIRUS)
						Console.WriteLine("Found: " + result.VirusName);
					else
						Console.WriteLine("File Clean!");
				}
			} //engine is disposed of here and the allocated engine freed
		}
	}
}
