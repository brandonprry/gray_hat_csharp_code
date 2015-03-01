using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ch4_bind_udp
{
	public class Client
	{
		static void Main(string[] args)
		{
			int lport = int.Parse(args[1]);
			UdpClient listener = new UdpClient(lport);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, lport);
			string output;
			byte[] bytes;

			Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
				ProtocolType.Udp);

			IPAddress addr = IPAddress.Parse(args[0]);
			IPEndPoint addrEP = new IPEndPoint(addr, lport);

			Console.WriteLine("Enter command to send, blank line to quit");
			while (true)
			{
				string command = Console.ReadLine();

				byte[] buff = Encoding.ASCII.GetBytes(command);
				try
				{
					sock.SendTo(buff, addrEP);

					if (command.Length == 0)
						break;

					bytes = listener.Receive(ref groupEP);
					output = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
					Console.WriteLine(output);
				}
				catch (Exception ex )
				{
					Console.WriteLine(" Exception {0}", ex.Message);
				}
			} 
			listener.Close ();
		} 
	}
}

