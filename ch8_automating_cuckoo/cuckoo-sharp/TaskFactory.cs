using System;
using Newtonsoft.Json.Linq;

namespace cuckoosharp
{
	public static class TaskFactory
	{
		public static Task CreateTask(JToken dict)
		{
			Task task = null;
			switch((string)dict["category"])
			{
			case "file":
				task = new FileTask(dict);
				break;
			default:
				throw new Exception("Don't know category: " + dict["category"]);
			}
			
			return task;
		}
	}
}

