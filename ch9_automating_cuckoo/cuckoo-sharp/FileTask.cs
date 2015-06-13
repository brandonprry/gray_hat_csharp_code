using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace cuckoosharp
{
	public class FileTask : Task
	{
		public FileTask () : base(null)
		{
		}
		
		public FileTask  (JToken dict) : base(dict)
		{
		}
		
		public string Filepath { get; set; }
	}
}

