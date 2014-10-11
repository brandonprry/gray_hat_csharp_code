using System;
using System.Runtime.InteropServices;

namespace clamsharp
{
	public class ClamEngine : IDisposable
	{
		private ClamEngineDescriptor engine;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="clamsharp.ClamEngine"/> class.
		/// 
		/// This class drives the file scanning and reporting. It implements IDisposable, and
		/// is meant to be used in the context of a using statement. If the programmer does not
		/// do this, the engine allocated by libclamav will not be freed and this will lead to
		/// a memory leak. If the programmer is using a C# prior to .NET 2.0 profile, the programmer
		/// should call Dispose explicitly when done.
		/// </summary>
		/// <exception cref='Exception'>
		/// Represents errors that occur during application execution.
		/// </exception>
		public ClamEngine ()
		{
			ClamReturnCode ret = ClamBindings.cl_init((uint)ClamDatabaseOptions.CL_DB_STDOPT);
			
			if (ret != ClamReturnCode.CL_SUCCESS)
				throw new Exception("Expected CL_SUCCESS, got " + ret);
			
			engine = ClamBindings.cl_engine_new();
			
			string dbDir = Marshal.PtrToStringAnsi(ClamBindings.cl_retdbdir());
			uint signo = 0;
			
			ret = ClamBindings.cl_load(dbDir, engine, ref signo,(uint)ClamScanOptions.CL_SCAN_STDOPT);
			
			if (ret != ClamReturnCode.CL_SUCCESS)
				throw new Exception("Expected CL_SUCCESS, got " + ret);
			
			ret = (ClamReturnCode)ClamBindings.cl_engine_compile(engine);
			
			if (ret != ClamReturnCode.CL_SUCCESS)
				throw new Exception("Expected CL_SUCCESS, got " + ret);
		}
		
		/// <summary>
		/// Scan a file with the standard options.
		/// </summary>
		/// <returns>
		/// The result of the scan, a ClamResult object or null.
		/// </returns>
		/// <param name='filepath'>
		/// The path to the file to be scanned
		/// </param>
		public ClamResult ScanFile(string filepath)
		{
			return this.ScanFile(filepath, (uint)ClamScanOptions.CL_SCAN_STDOPT);
		}
		
		/// <summary>
		/// This method will scan a given file using the options passed in.
		/// 
		/// It will return a ClamResult, which can be null if no virus is found.
		/// </summary>
		/// <returns>
		/// A ClamResult object (or null).
		/// </returns>
		/// <param name='filepath'>
		/// The path to the file to be scanned.
		/// </param>
		/// <param name='options'>
		/// The options to perform the scan with.
		/// </param>
		public ClamResult ScanFile(string filepath, uint options)
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
				return null;
			else if (ret == ClamReturnCode.CL_EOPEN)
				Console.WriteLine("Could not open " + filepath);
			else
				throw new Exception("Expected either CL_CLEAN or CL_VIRUS, got: " + ret);

			return null;
		}
		
		/// <summary>
		/// Releases all resource used by the <see cref="clamsharp.ClamEngine"/> object.
		/// </summary>
		/// <remarks>
		/// Call <see cref="Dispose"/> when you are finished using the <see cref="clamsharp.ClamEngine"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="clamsharp.ClamEngine"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="clamsharp.ClamEngine"/> so the garbage
		/// collector can reclaim the memory that the <see cref="clamsharp.ClamEngine"/> was occupying.
		/// </remarks>
		/// <exception cref='Exception'>
		/// Represents errors that occur during application execution.
		/// </exception>
		public void Dispose()
		{
			ClamReturnCode ret = ClamBindings.cl_engine_free(engine);
			
			if (ret != ClamReturnCode.CL_SUCCESS)
				throw new Exception("Expected CL_SUCCESS, got " + ret);
		}
	}
}