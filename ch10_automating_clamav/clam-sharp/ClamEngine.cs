using System;
using System.Runtime.InteropServices;

namespace ch13_automating_clamav_filesystem
{
	public class ClamEngine : IDisposable
	{
		private IntPtr engine;

		public ClamEngine ()
		{
			ClamReturnCode ret = ClamBindings.cl_init((uint)ClamDatabaseOptions.CL_DB_STDOPT);
			
			if (ret != ClamReturnCode.CL_SUCCESS)
				throw new Exception("Expected CL_SUCCESS, got " + ret);
			
			engine = ClamBindings.cl_engine_new();

			try
			{
				string dbDir = Marshal.PtrToStringAnsi(ClamBindings.cl_retdbdir());
				uint signo = 0;

				ret = ClamBindings.cl_load(dbDir, engine, ref signo, (uint)ClamScanOptions.CL_SCAN_STDOPT);

				if (ret != ClamReturnCode.CL_SUCCESS)
					throw new Exception("Expected CL_SUCCESS, got " + ret);

				ret = (ClamReturnCode)ClamBindings.cl_engine_compile(engine);

				if (ret != ClamReturnCode.CL_SUCCESS)
					throw new Exception("Expected CL_SUCCESS, got " + ret);
			}
			catch
			{
				ret = ClamBindings.cl_engine_free(engine);

				if (ret != ClamReturnCode.CL_SUCCESS)
					Console.Error.WriteLine("Freeing allocated engine failed");
			}
		}

		public ClamResult ScanFile(string filepath, uint options = (uint)ClamScanOptions.CL_SCAN_STDOPT)
		{
			ulong scanned = 0;
			IntPtr vname = (IntPtr)null;
			ClamReturnCode ret = ClamBindings.cl_scanfile(filepath, ref vname, ref scanned, engine, options);

			if (ret == ClamReturnCode.CL_VIRUS)
			{
				string virus = Marshal.PtrToStringAnsi(vname);

				ClamResult result = new ClamResult();
				result.ReturnCode = ret;
				result.VirusName = virus;
				result.FullPath = filepath;

				return result;
			}
			else if (ret == ClamReturnCode.CL_CLEAN)
				return new ClamResult() { ReturnCode = ret };
			else
				throw new Exception ("Expected either CL_CLEAN or CL_VIRUS, got: " + ret);
		}

		public void Dispose()
		{
			ClamReturnCode ret = ClamBindings.cl_engine_free(engine);
			
			if (ret != ClamReturnCode.CL_SUCCESS)
				Console.Error.WriteLine("Freeing allocated engine failed");
		}
	}
}