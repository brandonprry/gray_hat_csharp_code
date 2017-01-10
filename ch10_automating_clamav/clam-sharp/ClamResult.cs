using System;

namespace ch13_automating_clamav_filesystem
{
	public class ClamResult
	{
		public ClamReturnCode ReturnCode { get; set; }
		public string VirusName { get; set; }
		public string FullPath { get; set; }
	}
}

