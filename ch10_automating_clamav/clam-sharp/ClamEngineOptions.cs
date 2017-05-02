using System;

namespace ch13_automating_clamav_filesystem
{
	[Flags]
	public enum ClamDatabaseOptions
	{
		CL_DB_PHISHING = 0x2,
		CL_DB_PHISHING_URLS = 0x8,
		CL_DB_BYTECODE = 0x2000,
		CL_DB_STDOPT = (CL_DB_PHISHING | CL_DB_PHISHING_URLS | CL_DB_BYTECODE),
	}
}

