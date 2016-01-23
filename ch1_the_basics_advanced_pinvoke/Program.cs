using System;
using System.Runtime.InteropServices;

namespace ch1_the_basics_advanced_pinvoke
{
	class MainClass
	{
		[DllImport("user32", CharSet=CharSet.Auto)]
		static extern int MessageBox(IntPtr hWnd, 
			String text, String caption, int options);

		[DllImport("libc", EntryPoint="printf")]
		static extern void printf(string message);

		static void Main(string[] args)
		{
			OperatingSystem os = Environment.OSVersion;

			if (os.Platform == PlatformID.Win32Windows 
				|| os.Platform == PlatformID.Win32NT) {

				MessageBox (IntPtr.Zero, "Hello world!", "Hello world!", 0);
			} else {
				printf ("Hello world!");
			}
		}
	}
}
