using System;

namespace ch13_automating_clamav_filesystem
{
	public enum ClamCountSigsOptions
	{
		CL_COUNTSIGS_OFFICIAL = 0x1,
		CL_COUNTSIGS_UNOFFICIAL = 0x2,
		CL_COUNTSIGS_ALL = (CL_COUNTSIGS_OFFICIAL | CL_COUNTSIGS_UNOFFICIAL)
	}
}

