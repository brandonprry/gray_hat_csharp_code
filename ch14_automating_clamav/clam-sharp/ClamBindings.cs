using System;
using System.Runtime.InteropServices;

namespace ch13_automating_clamav_filesystem
{
	public static class ClamBindings
	{
		const string ClamLibPath = "clamav";

		[DllImport(ClamLibPath)]
		public extern static ClamReturnCode cl_init(uint options);

		[DllImport(ClamLibPath)]
		public extern static IntPtr cl_engine_new();

		[DllImport(ClamLibPath)]
		public extern static ClamReturnCode cl_engine_free(IntPtr engine);

		[DllImport(ClamLibPath)]
		public extern static IntPtr cl_retdbdir();

		[DllImport(ClamLibPath)]
		public extern static ClamReturnCode cl_load(string path, IntPtr engine, ref uint signo, uint options);

		[DllImport(ClamLibPath)]
		public extern static ClamReturnCode cl_scanfile(string path, ref IntPtr virusName, ref ulong scanned, IntPtr engine, uint options);

		[DllImport(ClamLibPath)]
		public extern static ClamReturnCode cl_engine_compile(IntPtr engine);
	}
}

