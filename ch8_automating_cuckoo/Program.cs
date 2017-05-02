using System;
using cuckoosharp;
using Newtonsoft.Json.Linq;

namespace Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			CuckooSession session = new CuckooSession ("192.168.1.105", 8090);

			JObject response = session.ExecuteCommand ("/cuckoo/status", "GET");
			Console.WriteLine(response.ToString());

			using (CuckooManager manager = new CuckooManager(session))
			{
				FileTask task = new FileTask();
				task.Filepath = "/Users/bperry/Projects/metasploit-framework/data/post/bypassuac-x64.exe";

				int taskID = manager.CreateTask(task);
				task = (FileTask)manager.GetTaskDetails(taskID);
				while(task.Status == "pending" || task.Status == "running")
				{
					Console.WriteLine("Waiting 30 seconds..."+task.Status);
					System.Threading.Thread.Sleep(30000);
					task = (FileTask)manager.GetTaskDetails(taskID);
				}

				if (task.Status == "failure")
				{
					Console.WriteLine ("There was an error:");
					foreach (var error in task.Errors)
						Console.WriteLine(error);

					return;
				}

				string report = manager.GetTaskReport(taskID).ToString();

				Console.WriteLine(report);
			}
		}
	}
}
