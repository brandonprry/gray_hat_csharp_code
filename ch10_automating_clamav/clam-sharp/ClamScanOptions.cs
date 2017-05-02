using System;

namespace ch13_automating_clamav_filesystem
{
	[Flags]
	public enum ClamScanOptions
	{
		CL_SCAN_ARCHIVE = 0x1,
		CL_SCAN_MAIL = 0x2,
		CL_SCAN_OLE2 = 0x4,
		CL_SCAN_HTML = 0x10,
		CL_SCAN_PE = 0x20,
		CL_SCAN_ALGORITHMIC = 0x200,
		CL_SCAN_ELF = 0x2000,
		CL_SCAN_PDF = 0x4000,
		CL_SCAN_STDOPT = (CL_SCAN_ARCHIVE | CL_SCAN_MAIL | CL_SCAN_OLE2 | CL_SCAN_PDF | CL_SCAN_HTML | CL_SCAN_PE | CL_SCAN_ALGORITHMIC | CL_SCAN_ELF)
	}
}

