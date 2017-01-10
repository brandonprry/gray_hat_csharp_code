using System;

namespace ch13_automating_clamav_filesystem
{
	public enum ClamReturnCode
	{
		CL_CLEAN = 0x0,
		CL_SUCCESS = 0x0,
		CL_VIRUS = 0x1
	}
}

