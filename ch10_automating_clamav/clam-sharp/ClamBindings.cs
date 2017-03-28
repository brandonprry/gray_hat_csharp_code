using System;
using System.Runtime.InteropServices;

namespace ch13_automating_clamav_filesystem
{
	static class ClamBindings
	{
		const string _clamLibPath = "/Users/bperry/tmp/clamav-0.99/libclamav/.libs/libclamav.7.dylib";

		[DllImport(_clamLibPath)]
		public extern static ClamReturnCode cl_init(uint options);

		[DllImport(_clamLibPath)]
		public extern static IntPtr cl_engine_new();

		[DllImport(_clamLibPath)]
		public extern static ClamReturnCode cl_engine_free(IntPtr engine);

		[DllImport(_clamLibPath)]
		public extern static IntPtr cl_retdbdir();

		[DllImport(_clamLibPath)]
		public extern static ClamReturnCode cl_load(string path, IntPtr engine, ref uint signo, uint options);

		[DllImport(_clamLibPath)]
		public extern static ClamReturnCode cl_scanfile(string path, ref IntPtr virusName, ref ulong scanned, IntPtr engine, uint options);

		[DllImport(_clamLibPath)]
		public extern static ClamReturnCode cl_engine_compile(IntPtr engine);
	}
}

