using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace cuckoosharp
{
	public class URLTask : Task
	{
		public URLTask () : base(null)
		{
		}
		
		public URLTask (JToken dict) : base(dict)
		{
		}
		
		public string URL { get; set; }
	}
}

