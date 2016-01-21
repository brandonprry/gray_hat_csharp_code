using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Sockets;
using MsgPack.Serialization;
using MsgPack;
using System.Collections.Specialized;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace ch13_automating_metasploit
{
	public class MetasploitSession : IDisposable
	{
		string _host;
		string _token;

		public MetasploitSession (string username, string password, string host)
		{
			ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => {
				return true;
			};

			_host = host;
			_token = null;
			
			Dictionary<object, object> response = this.Authenticate (username, password);
			
			bool loggedIn = !response.ContainsKey ("error");

			if (!loggedIn)
				throw new Exception (response ["error_message"] as string);
			
			if ((response ["result"] as string) == "success")
				_token = response ["token"] as string;
		}

		public string Token { 
			get { return _token; }
		}

		public Dictionary<object, object> Authenticate (string username, string password)
		{
			return this.Execute ("auth.login", username, password);
		}

		public Dictionary<object, object> Execute (string method, params object[] args)
		{
			if (string.IsNullOrEmpty (_host))
				throw new Exception ("Host null or empty");
			
			if (method != "auth.login" && string.IsNullOrEmpty (_token))
				throw new Exception ("Not authenticated.");
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (_host);
			request.ContentType = "binary/message-pack";
			request.Method = "POST";
			request.KeepAlive = true;

			Stream requestStream = null;

			try {
				requestStream = request.GetRequestStream ();
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}

			Packer msgpackWriter = Packer.Create (requestStream);
			msgpackWriter.PackArrayHeader (args.Length + 1 + (string.IsNullOrEmpty (_token) ? 0 : 1));
			msgpackWriter.Pack (method);
			
			if (!string.IsNullOrEmpty (_token) && method != "auth.login")
				msgpackWriter.Pack (_token);
			
			foreach (object arg in args)
				msgpackWriter.Pack (arg);
			
			requestStream.Close ();

			byte[] buffer = new byte[4096];
			MemoryStream mstream = new MemoryStream ();
			try {
				using (WebResponse response = request.GetResponse ()) {
					using (Stream rstream = response.GetResponseStream ()) {
						int count = 0;
					
						do {
							count = rstream.Read (buffer, 0, buffer.Length);
							mstream.Write (buffer, 0, count);
						} while (count != 0);
					
					}
				}
			} catch (WebException ex) {
				if (ex.Response != null) {
					string res = string.Empty;
					using (StreamReader rdr = new StreamReader (ex.Response.GetResponseStream ()))
						res = rdr.ReadToEnd ();

					Console.WriteLine (res);
				}
			}
			
			mstream.Position = 0;

			MessagePackObjectDictionary resp = Unpacking.UnpackObject (mstream).AsDictionary ();
			return MessagePackToDictionary (resp);
		}

		private Dictionary<object, object> MessagePackToDictionary (MessagePackObjectDictionary dict)
		{
			Dictionary<object, object> newDict = new Dictionary<object, object> ();
			foreach (var pair in dict) {
				object newKey = GetObject (pair.Key);
				if (pair.Value.IsTypeOf<MessagePackObjectDictionary> () == true)
					newDict [newKey] = MessagePackToDictionary (pair.Value.AsDictionary ());
				else
					newDict [newKey] = GetObject (pair.Value);
			}

			return newDict;
		}

		private object GetObject (MessagePackObject str)
		{
			if (str.UnderlyingType == typeof(byte[]))
				return System.Text.Encoding.ASCII.GetString (str.AsBinary ());
			else if (str.UnderlyingType == typeof(string))
				return str.AsString ();
			else if (str.UnderlyingType == typeof(byte))
				return str.AsByte ();
			else if (str.UnderlyingType == typeof(bool))
				return str.AsBoolean ();

			return str;
		}

		public void Dispose ()
		{
			if (this.Token != null) {
				this.Execute ("auth.logout", new object[] { });
				_token = null;
			}
		}
	}
}

