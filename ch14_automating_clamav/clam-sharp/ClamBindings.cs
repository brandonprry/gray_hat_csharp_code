using System;
using System.Runtime.InteropServices;

namespace clamsharp
{
	/// <summary>
	/// This class is not meant to be instantiated *ever*.
	/// 
	/// This class is the glue between libclamav and our managed code.
	/// </summary>
	public static class ClamBindings
	{
		/// <summary>
		/// This function is required in order to instantiate and use a ClamEngine.
		/// 
		/// It will return CL_SUCCESS when initiation is successful, or the respective 
		/// return code when the initiation fails.
		/// </summary>
		/// <param name='options'>
		/// Options.
		/// </param>
		[DllImport("clamav.so.6")]
		public extern static ClamReturnCode cl_init(uint options);
		
		/// <summary>
		/// This function allocates a new engine and returns a descriptor that will
		/// be passed to the other functions that require an engine. It will return 
		/// CL_SUCCESS on success, or the respective return code when allocation fails.
		/// </summary>
		[DllImport("clamav.so.6")]
		public extern static ClamEngineDescriptor cl_engine_new();
		
		/// <summary>
		/// This function frees the previously allocated engine.
		/// It will return CL_SUCCESS on success, or the respective return code when it fails.
		/// </summary>
		/// <param name='engine'>
		/// Engine.
		/// </param>
		[DllImport("clamav.so.6")]
		public extern static ClamReturnCode cl_engine_free(ClamEngineDescriptor engine);
		
		/// <summary>
		/// This function return a pointer to a string that is the default database path
		/// for clamav. This is hardcoded to /var/lib/clamav.
		/// 
		/// The programmer should use Marshal.PtrToStringAnsi to get the real string and not the IntPtr.
		/// </summary>
		[DllImport("clamav.so.6")]
		public extern static IntPtr cl_retdbdir();
		
		/// <summary>
		/// This function loads the database into the given engine (pre-allocated with cl_engine_new().
		/// 
		/// Generally, the programmer will pass in the database path return by cl_retdbdir() as the directory path.
		/// 
		/// Will return CL_SUCCESS on a successful database load, or the respective return code on error.
		/// </summary>
		/// <param name='path'>
		/// Path to the database(s) you want to load
		/// </param>
		/// <param name='engine'>
		/// The pre-allocated engine allocated with cl_engine_new()
		/// </param>
		/// <param name='signo'>
		/// The number of signatures loaded.
		/// </param>
		/// <param name='options'>
		/// The database options specifying which databases to load or ignore.
		/// </param>
		[DllImport("clamav.so.6")]
		public extern static ClamReturnCode cl_load(string path, ClamEngineDescriptor engine, ref uint signo, uint options);
		
		/// <summary>
		/// This function will scan a specific file using a pre-allocated engine (allocated with cl_engine_new().
		/// 
		/// This function return CL_VIRUS when a virus is found or CL_CLEAN when the file is clean.
		/// It will return the respective error code whne the scan fails for whatever reason.
		/// </summary>
		/// <param name='path'>
		/// Path to the file to be scanned.
		/// </param>
		/// <param name='virusName'>
		/// If the file is found to be infected, this pointer will point to a string that contains the name of the virus found in the file.
		/// 
		/// The programmer should use Marshal.PtrToStringAnsi() in order to get the string from the pointer itself.
		/// </param>
		/// <param name='scanned'>
		/// Scanned.
		/// </param>
		/// <param name='engine'>
		/// The engine that was previously allocated with cl_engine_new()
		/// </param>
		/// <param name='options'>
		/// The scan options to be used when scanning the file.
		/// </param>
		[DllImport("clamav.so.6")]
		public extern static ClamReturnCode cl_scanfile(string path, ref IntPtr virusName, ref ulong scanned, ClamEngineDescriptor engine, uint options);
		
		/// <summary>
		/// Compile the engine after allocating it.
		/// </summary>
		/// <param name='engine'>
		/// The previously allocated engine (allocated with cl_engine_new())
		/// </param>
		[DllImport("clamav.so.6")]
		public extern static ClamReturnCode cl_engine_compile(ClamEngineDescriptor engine);
	}
}

