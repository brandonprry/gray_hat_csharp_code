using System;
using clamdsharp;

namespace ch13_automating_clamav_clamd
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ClamdSession session = new ClamdSession ("127.0.0.1", 3310);
			ClamdManager manager = new ClamdManager (session);

			Console.WriteLine(manager.GetVersion());
			Console.WriteLine(manager.Scan("/Users/bperry/eicar.com.txt"));
		}
	}
}
