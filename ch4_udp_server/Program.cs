using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace ch4_udp_server
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			int lport = int.Parse (args [0]);
			using (UdpClient listener = new UdpClient (lport)) {
				IPEndPoint localEP = new IPEndPoint (IPAddress.Any, lport);
				string cmd;
				byte[] input;

				while (true) {
					input = listener.Receive (ref localEP);
					cmd = Encoding.ASCII.GetString (input, 0, input.Length);

					if (string.IsNullOrEmpty (cmd)) {
						listener.Close ();
						return;
					}

					if (string.IsNullOrWhiteSpace (cmd))
						continue;

					string[] split = cmd.Trim ().Split (' ');
					string filename = split.First ();
					string arg = string.Join (" ", split.Skip (1));
					string results = string.Empty;

					try {
						Process prc = new Process ();
						prc.StartInfo = new ProcessStartInfo ();
						prc.StartInfo.FileName = filename;
						prc.StartInfo.Arguments = arg;
						prc.StartInfo.UseShellExecute = false;
						prc.StartInfo.RedirectStandardOutput = true;
						prc.Start ();
						prc.WaitForExit ();

						results = prc.StandardOutput.ReadToEnd ();
					} catch {
						results = "There was an error running the command: " + filename;
					}

					using (Socket sock = new Socket (AddressFamily.InterNetwork, SocketType.Dgram,
						ProtocolType.Udp)) {

						IPAddress sender = localEP.Address;
						IPEndPoint remoteEP = new IPEndPoint (sender, lport);

						byte[] resultsBytes = Encoding.ASCII.GetBytes (results);
						sock.SendTo (resultsBytes, remoteEP);
					}
				}
			}
		}
	}
}
