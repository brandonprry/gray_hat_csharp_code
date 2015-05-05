using System;
using System.Net.Sockets;
using System.IO;

namespace clamdsharp
{
	public class ClamdSession : IDisposable
	{
		private string _host = null;
		private int _port;

		public ClamdSession (string host, int port) 
		{
			_host = host;
			_port = port;
		}

		public string Execute (string command)
		{
			string resp = string.Empty;
			using (NetworkStream stream = new TcpClient(_host, _port).GetStream()) {
				byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
				stream.Write(data, 0, data.Length);

				using (StreamReader rdr = new StreamReader(stream))
					resp = rdr.ReadToEnd();
			}

			return resp;
		}

		public void Dispose() {}
	}
}

