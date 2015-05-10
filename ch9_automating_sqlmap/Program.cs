using System;
using sqlmapsharp;
using System.Collections.Generic;

namespace ch9_automating_sqlmap
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (SqlmapSession session = new SqlmapSession("127.0.0.1", 8775)) 
			{ 
				using (SqlmapManager manager = new SqlmapManager(session)) 
				{ 
					string taskid = manager.NewTask();

					Dictionary<string, object> options = manager.GetOptions(taskid); 
					options["url"] = args[0]; 

					manager.StartTask(taskid, options); 

					SqlmapStatus status = manager.GetScanStatus(taskid); 
					while (status.Status != "terminated") { 
						System.Threading.Thread.Sleep(new TimeSpan(0,0,10)); 
						status = manager.GetScanStatus(taskid); 
					} 

					List<SqlmapLogItem> logItems = manager.GetLog(taskid); 
					foreach (SqlmapLogItem item in logItems) 
						Console.WriteLine(item.Message); 

					manager.DeleteTask(taskid); 
				} 
			} 
		} 
	}
}
