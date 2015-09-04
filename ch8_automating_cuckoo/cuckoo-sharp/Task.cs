using System;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace cuckoosharp
{
	public abstract class Task
	{	
		public Task(JToken resp)
		{
			if (resp != null)
			{
				this.AddedOn = DateTime.Parse((string)resp["added_on"]);
				
				if (resp["completed_on"].Type != JTokenType.Null)
					this.CompletedOn = DateTime.Parse(resp["completed_on"].ToObject<string>());
				
				this.Machine = (string)resp["machine"];
				this.Errors = resp["errors"].ToObject<ArrayList>();
				this.Custom = (string)resp["custom"];
				this.EnableEnforceTimeout = (bool)resp["enforce_timeout"];
				this.EnableMemoryDump = (bool)resp["memory"];
				this.Guest = resp["guest"];
				this.ID = (int)resp["id"];
				this.Options = (string)resp["options"];
				this.Package = (string)resp["package"];
				this.Platform = (string)resp["platform"];
				this.Priority = (int)resp["priority"];
				this.SampleID = (int)resp["sample_id"];
				this.Status = (string)resp["status"];
				this.Target = (string)resp["target"];
				this.Timeout = (int)resp["timeout"];
			}
		}
		
		public string Package { get; set; }
		
		public int Timeout { get; set; }
		
		public string Options { get; set; }
		
		public string Machine { get; set; }
		
		public string Platform { get; set; }
		
		public string Custom { get; set; }
		
		public bool EnableMemoryDump { get; set; }
		
		public bool EnableEnforceTimeout { get; set; }
		
		public ArrayList Errors { get; set; }
		
		public string Target { get; set; }
		
		public int SampleID { get; set; }
		
		public JToken Guest { get; set; }
		
		public int Priority { get; set; }
		
		public string Status { get; set; }
		
		public int ID { get; set; }
		
		public DateTime AddedOn { get; set; }
		
		public DateTime CompletedOn { get; set; }
		
	}
}

