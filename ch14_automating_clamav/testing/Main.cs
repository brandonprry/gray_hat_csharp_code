using System;
using clamsharp;
using clamdsharp;

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

			//these test clamd bindings
			using (ClamdSession session = new ClamdSession("127.0.0.1", 3310))
			{
				using (ClamdManager manager = new ClamdManager(session))
				{
					Console.WriteLine(manager.GetVersion());
					Console.WriteLine(manager.ScanWithArchiveSupport("/home/bperry/tmp"));
				}
			}
		}
	}
}
