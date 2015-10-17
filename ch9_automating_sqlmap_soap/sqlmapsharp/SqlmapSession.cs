using System;
using System.Net;

namespace sqlmapsharp
{
	public class SqlmapSession : IDisposable
	{
		private string _host = string.Empty;
		private int _port;

		public SqlmapSession (string host, int port)
		{
			_host = host;
			_port = port;
		}

		public string ExecuteGet(string url)
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create ("http://" + _host + ":" + _port + url);
			req.Method = "GET";

			string resp = string.Empty;
			using (System.IO.Stream stream = req.GetResponse().GetResponseStream()) {
				using (System.IO.StreamReader rdr = new System.IO.StreamReader(stream))
				{
					resp = rdr.ReadToEnd();
				}
			}

			return resp;
		}

		public string ExecutePost (string url, string data)
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create ("http://" + _host + ":" + _port + url);

			req.Method = "POST";
			req.ContentType = "application/json";

			byte[] datas = System.Text.Encoding.ASCII.GetBytes(data);

			req.ContentLength = datas.Length;

			using (System.IO.Stream stream = req.GetRequestStream()) {
				stream.Write(datas, 0, datas.Length);
			}

			string resp = string.Empty;

			using (System.IO.Stream stream = req.GetResponse().GetResponseStream()) {
				using (System.IO.StreamReader rdr = new System.IO.StreamReader(stream))
				{
					resp = rdr.ReadToEnd();
				}
			}

			return resp;
		}

		public void Dispose()
		{

		}
	}
}

