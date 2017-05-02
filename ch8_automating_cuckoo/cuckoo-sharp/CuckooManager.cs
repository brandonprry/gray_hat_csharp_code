using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace cuckoosharp
{
	public class CuckooManager : IDisposable
	{
		CuckooSession _session = null;
		public CuckooManager (CuckooSession session)
		{
			_session = session;
		}

		public int CreateTask(Task task)
		{
			string param = null, uri = "/tasks/create/";
			object val = null;

			if (task is FileTask)
			{
				byte[] data;
				using (FileStream str = new FileStream((task as FileTask).Filepath, FileMode.Open, FileAccess.Read))
				{
					data = new byte[str.Length];
					str.Read(data, 0, data.Length);
				}
				param = "file";
				uri += param;
				val = new FileParameter(data, (task as FileTask).Filepath, "application/binary");

			}

			IDictionary<string, object> parms = new Dictionary<string, object>();

			parms.Add(param, val);
			parms.Add("package", task.Package);
			parms.Add("timeout", task.Timeout.ToString());
			parms.Add("options", task.Options);
			parms.Add("machine", task.Machine);
			parms.Add("platform", task.Platform);
			parms.Add("custom", task.Custom);
			parms.Add("memory", task.EnableMemoryDump.ToString());
			parms.Add("enforce_timeout", task.EnableEnforceTimeout.ToString());

			JObject resp = _session.ExecuteCommand(uri, "POST", parms);

			return (int)resp["task_id"];
		}
		
		public Task GetTaskDetails(int id)
		{
			string uri = "/tasks/view/" + id;
			JObject resp = _session.ExecuteCommand(uri, "GET");
			
			return TaskFactory.CreateTask(resp["task"]);
		}
		
		public JObject GetTaskReport(int id)
		{
			return GetTaskReport(id, "json");
		}
		
		public JObject GetTaskReport(int id, string type)
		{
			string uri = "/tasks/report/" + id + "/" + type;
			JObject resp = _session.ExecuteCommand(uri, "GET");
			
			return resp;
		}
			
		public void Dispose()
		{
			_session = null;
		}
	}
}