using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ch4_bind_udp
{
	public class Client
	{
		public Client ()
		{
		}

		static void Main(string[] args)
		{
			UdpClient listener = new UdpClient(11000);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 11000);
			string received_data;
			byte[] receive_byte_array;

			Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
				ProtocolType.Udp);

			IPAddress send_to_address = IPAddress.Parse("192.168.1.31");

			IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 11000);

			Console.WriteLine("Enter text to broadcast via UDP.");
			Console.WriteLine("Enter a blank line to exit the program.");
			while (true)
			{
				Console.WriteLine("Enter text to send, blank line to quit");
				string text_to_send = Console.ReadLine();


				byte[] send_buffer = Encoding.ASCII.GetBytes(text_to_send);

				Console.WriteLine("sending to address: {0} port: {1}", 
					sending_end_point.Address, 
					sending_end_point.Port );
				try
				{
					sending_socket.SendTo(send_buffer, sending_end_point);

					if (text_to_send.Length == 0)
						break;

					Console.WriteLine("Waiting for broadcast");
					receive_byte_array = listener.Receive(ref groupEP);
					Console.WriteLine("Received a broadcast from {0}", groupEP.ToString() );
					received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
					Console.WriteLine("data follows \n{0}\n\n", received_data);
				}
				catch (Exception send_exception )
				{
					Console.WriteLine(" Exception {0}", send_exception.Message);
				}
			} 
		} 
	}
}

