using System;

namespace clamsharp
{
	/// <summary>
	/// This class encompasses the relevant information returned when a file is scanned.
	/// </summary>
	public class ClamResult
	{
		public ClamReturnCode ReturnCode { get; set; }
		
		public string VirusName { get; set; }
		
		public string FullPath { get; set; }
	}
}

