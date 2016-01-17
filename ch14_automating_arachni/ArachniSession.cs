using System;
using System.Net.Security;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json.Linq;

namespace ch14_automating_arachni
{
	public class ArachniSession : IDisposable
	{
		public ArachniSession (string host, int port)
		{
			this.Host = host;
			this.Port = port;
		}

		public string Host { get; set; }
		public int Port { get; set; }

		public JObject ExecuteRequest(string method, string uri, JObject data = null)
		{
			string url = "http://" + this.Host + ":" + this.Port.ToString () + uri;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			request.Method = method;

			if (data != null) {
				string dataString = data.ToString ();
				byte[] dataBytes = System.Text.Encoding.ASCII.GetBytes (dataString);

				request.ContentType = "application/json";
				request.ContentLength = dataBytes.Length;

				request.GetRequestStream ().Write (dataBytes, 0, dataBytes.Length);
			}

			string resp = string.Empty;
			using (StreamReader reader = new StreamReader (request.GetResponse ().GetResponseStream ()))
				resp = reader.ReadToEnd ();

			return JObject.Parse (resp);
		}

		public void Dispose ()
		{
		}
	}
}

