using System;

namespace cuckoosharp
{
	public class Machine
	{
		public Machine ()
		{
		}
		
		public string Status { get; set; }
		
		public bool Locked { get; set; }
		
		public string Name { get; set; }
		
		public string IP { get; set; }
		
		public string Label { get; set; }
		
		public DateTime LockedChangedOn { get; set; }
		
		public string Platform { get; set; }
		
		public DateTime StatusChangedOn { get; set; }
		
		public int ID { get; set; }
	}
}

