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
			using (UdpClient listener = new UdpClient (lport)) {
				IPEndPoint remoteEP = new IPEndPoint (IPAddress.Any, lport);
				string output;
				byte[] bytes;

				using (Socket sock = new Socket (AddressFamily.InterNetwork, SocketType.Dgram,
					                     ProtocolType.Udp)) {

					IPAddress addr = IPAddress.Parse (args [0]);
					IPEndPoint addrEP = new IPEndPoint (addr, lport);

					Console.WriteLine ("Enter command to send, blank line to quit");
					while (true) {
						string command = Console.ReadLine ();

						byte[] buff = Encoding.ASCII.GetBytes (command);
						try {
							sock.SendTo (buff, addrEP);

							if (string.IsNullOrEmpty (command)) {
								sock.Close();
								listener.Close();
								return;
							}

							if (string.IsNullOrWhiteSpace(command))
								continue;

							bytes = listener.Receive (ref remoteEP);
							output = Encoding.ASCII.GetString (bytes, 0, bytes.Length);
							Console.WriteLine (output);
						} catch (Exception ex) {
							Console.WriteLine (" Exception {0}", ex.Message);
						}
					} 
				}
			}
		} 
	}
}

