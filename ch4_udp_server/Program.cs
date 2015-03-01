using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace ch4_udp_server
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			int lport = int.Parse(args[1]);
			UdpClient listener = new UdpClient(lport);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, lport);
			string cmd;
			byte[] input;

			try
			{
				while (true)
				{
					input = listener.Receive(ref groupEP);
					cmd = Encoding.ASCII.GetString(input, 0, input.Length);

					if (cmd == string.Empty)
						break;
						
					string filename = string.Empty;
					string arg = string.Empty;

					if (cmd.IndexOf(' ') > 0) { 
						filename = cmd.Substring(0, cmd.IndexOf(' '));
						arg = cmd.Substring(cmd.IndexOf(' '), cmd.Length - filename.Length);
					} else {
						filename = cmd;
					}

					Process prc = new Process();
					prc.StartInfo = new ProcessStartInfo();
					prc.StartInfo.FileName = filename;
					prc.StartInfo.Arguments = arg;
					prc.StartInfo.UseShellExecute = false;
					prc.StartInfo.RedirectStandardOutput = true;
					prc.Start();
					prc.WaitForExit();

					Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
						ProtocolType.Udp);

					IPAddress sender = IPAddress.Parse(args[0]);
					IPEndPoint sendEP = new IPEndPoint(sender, lport);

					byte[] results = Encoding.ASCII.GetBytes(prc.StandardOutput.ReadToEnd());
					sock.SendTo(results, sendEP);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			listener.Close();
		}
	}
}
