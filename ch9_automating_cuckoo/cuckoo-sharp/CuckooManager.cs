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
					str.Read (data, 0, data.Length);
				}
				
				uri += (param = "file");
				val = new FileParameter(data, (task as FileTask).Filepath, "application/binary");
				
			}
			else if (task is URLTask)
			{
				uri += (param = "url");
				val = (task as URLTask).URL;
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
		
		public List<Task> GetTasks(int limit)
		{
			string uri = "/tasks/list/" + (limit == -1 ? string.Empty : limit.ToString());
			JObject resp = _session.ExecuteCommand(uri, "GET");
			
			List<Task> tasks = new List<Task>();
			foreach (JToken obj in resp["tasks"])
				tasks.Add(TaskFactory.CreateTask(obj));
			
			return tasks;
		}
		
		public List<Task> GetTasks()
		{
			return GetTasks(-1);
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
		
		public Sample GetFileDetails(int id)
		{
			string uri = "/files/view/id/" + id;
			JObject resp = _session.ExecuteCommand(uri, "GET");
			
			Sample sample = new Sample();
			
			sample.CRC32 = (string)resp["crc32"];
			sample.FileSize = (int)resp["file_size"];
			sample.FileType = (string)resp["file_type"];
			sample.ID = (int)resp["id"];
			sample.MD5 = (string)resp["md5"];
			sample.SHA1 = (string)resp["sha1"];
			sample.SHA256 = (string)resp["sha256"];
			sample.SHA512 = (string)resp["sha512"];
			sample.SSDeep = (string)resp["ssdeep"];
			
			return sample;
		}
		
		public Sample GetFileDetails(string hash, string hashType)
		{
			string uri = "/files/view/" + hashType + "/" + hash;
			JObject resp = _session.ExecuteCommand(uri, "GET");
			
			Sample sample = new Sample();
			
			sample.CRC32 = (string)resp["crc32"];
			sample.FileSize = (int)resp["file_size"];
			sample.FileType = (string)resp["file_type"];
			sample.ID = (int)resp["id"];
			sample.MD5 = (string)resp["md5"];
			sample.SHA1 = (string)resp["sha1"];
			sample.SHA256 = (string)resp["sha256"];
			sample.SHA512 = (string)resp["sha512"];
			sample.SSDeep = (string)resp["ssdeep"];
			
			return sample;
		}
		
		public string GetFile(int id)
		{
			return string.Empty;
		}
		
		public List<Machine> GetMachines()
		{
			string uri = "/machines/list";
			JObject resp = _session.ExecuteCommand(uri, "GET");
			
			List<Machine> machines = new List<Machine>();
			foreach (JToken obj in resp["machines"])
			{
				Machine machine = new Machine();
				machine.ID = (int)obj["id"];
				machine.IP = (string)obj["ip"];
				machine.Label = (string)obj["label"];
				machine.Locked = (bool)obj["locked"];
				
				if (obj["locked_changed_on"].Type != JTokenType.Null)
					machine.LockedChangedOn = DateTime.Parse(obj["locked_changed_on"].ToObject<string>());
				
				machine.Name = (string)obj["name"];
				machine.Platform = (string)obj["platform"];
				machine.Status = (string)obj["status"];
				
				if (obj["status_changed_on"].Type != JTokenType.Null)
					machine.StatusChangedOn = DateTime.Parse(obj["status_changed_on"].ToObject<string>());
				
				machines.Add(machine);
			}
			
			return machines;
		}
		
		public Machine GetMachineDetails(string name)
		{
			Machine machine = new Machine();
			string uri = "/machines/view/" + name;
			JObject resp = _session.ExecuteCommand(uri, "GET");	
		
			machine.ID = (int)resp["id"];
			machine.IP = (string)resp["ip"];
			machine.Label = (string)resp["label"];
			machine.Locked = (bool)resp["locked"];
			
			if (resp["locked_changed_on"].Type != JTokenType.Null)
				machine.LockedChangedOn = DateTime.Parse((string)resp["locked_changed_on"]);
			
			machine.Name = (string)resp["name"];
			machine.Platform = (string)resp["platform"];
			machine.Status = (string)resp["status"];
			
			if(resp["status_changed_on"].Type != JTokenType.Null)
				machine.StatusChangedOn = DateTime.Parse((string)resp["status_changed_on"]);
			
			return machine;
		}
		
		public void Dispose()
		{
			_session = null;
		}
	}
}