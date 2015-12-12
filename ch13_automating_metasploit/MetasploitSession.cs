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
			
			Dictionary<string, object > response = this.Authenticate (username, password);
			
			bool loggedIn = !response.ContainsKey ("error");

			if (!loggedIn)
				throw new Exception (response ["error_message"] as string);
			
			if ((response ["result"] as string) == "success")
				_token = response ["token"] as string;
		}

		public MetasploitSession (string token, string host)
		{
			_token = token;
			_host = host;
		}

		public string Token { 
			get { return _token; }
		}

		public Dictionary<string, object> Authenticate (string username, string password)
		{
			return this.Execute ("auth.login", username, password);
		}

		public Dictionary<string, object> Execute (string method, params object[] args)
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
			msgpackWriter.PackString (method);
			
			if (!string.IsNullOrEmpty (_token) && method != "auth.login")
				msgpackWriter.Pack (_token);
			
			foreach (object arg in args)
				Pack (msgpackWriter, arg);
			
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
			Dictionary<string, object > returnDictionary = TypifyDictionary (resp);
			return returnDictionary;
		}
			
		Dictionary<string, object> TypifyDictionary (MessagePackObjectDictionary dict)
		{
			Dictionary<string, object> returnDictionary = new Dictionary<string, object> ();
			
			foreach (var pair in dict) {
				MessagePackObject obj = (MessagePackObject)pair.Value;
				string key = System.Text.Encoding.ASCII.GetString ((byte[])pair.Key);

				if (obj.UnderlyingType == null)
					continue;
				
				if (obj.IsRaw) {
					if (obj.UnderlyingType == typeof(string)) {
						if (pair.Key.IsRaw && pair.Key.IsTypeOf (typeof(Byte[])).Value)
							returnDictionary [key] = obj.AsString ();
						else
							returnDictionary [pair.Key.ToString ()] = obj.AsString ();
					} else if (obj.IsTypeOf (typeof(int)).Value)
						returnDictionary [pair.Key.ToString ()] = (int)obj.ToObject ();
					else if (obj.IsTypeOf (typeof(Byte[])).Value) {
						if (key == "payload")
							returnDictionary [key] = (byte[])obj;
						else
							returnDictionary [key] = System.Text.Encoding.ASCII.GetString ((Byte[])obj.ToObject ());
					} else
						throw new Exception ("I don't know type: " + pair.Value.GetType ().Name);
				} else if (obj.IsArray) {
					List<object> arr = new List<object> ();
					foreach (var o in obj.ToObject() as MessagePackObject[]) {
						if (o.IsDictionary)
							arr.Add (TypifyDictionary (o.AsDictionary ()));
						else if (o.IsRaw)
							arr.Add (System.Text.Encoding.ASCII.GetString ((byte[])o));
						else if (o.IsArray) {
							var enu = o.AsEnumerable ();
							List<object> array = new List<object> ();
							foreach (var blah in enu)
								array.Add (blah as object);

							arr.Add (array.ToArray ());
						} else if (o.ToObject ().GetType () == typeof(Byte))
							arr.Add (o.ToString ());
					}

					if (pair.Key.IsRaw && pair.Key.IsTypeOf (typeof(Byte[])).Value)
						returnDictionary.Add (key, arr);
					else
						returnDictionary.Add (key, arr);
				} else if (obj.IsDictionary) {
					if (pair.Key.IsRaw && pair.Key.IsTypeOf (typeof(Byte[])).Value)
						returnDictionary [key] = TypifyDictionary (obj.AsDictionary ());
					else
						returnDictionary [pair.Key.ToString ()] = TypifyDictionary (obj.AsDictionary ());
				} else if (obj.IsTypeOf (typeof(UInt16)).Value) {
					if (pair.Key.IsRaw && pair.Key.IsTypeOf (typeof(Byte[])).Value)
						returnDictionary [key] = obj.AsUInt16 ();
					else
						returnDictionary [pair.Key.ToString ()] = obj.AsUInt16 ();
				} else if (obj.IsTypeOf (typeof(UInt32)).Value) {
					if (pair.Key.IsRaw && pair.Key.IsTypeOf (typeof(Byte[])).Value)
						returnDictionary [key] = obj.AsUInt32 ();
					else
						returnDictionary [pair.Key.ToString ()] = obj.AsUInt32 ();
				} else if (obj.IsTypeOf (typeof(bool)).Value) {
					if (pair.Key.IsRaw && pair.Key.IsTypeOf (typeof(Byte[])).Value)
						returnDictionary [key] = obj.AsBoolean ();
					else
						returnDictionary [pair.Key.ToString ()] = obj.AsBoolean ();
				} else
					throw new Exception ("Don't know type: " + obj.ToObject ().GetType ().Name);
			}
			
			return returnDictionary;
		}

		void Pack (Packer packer, object o)
		{
			if (o == null) {
				packer.PackNull ();
				return;
			}
 	
			if (o is int)
				packer.Pack ((int)o);
			else if (o is uint)
				packer.Pack ((uint)o);
			else if (o is float)
				packer.Pack ((float)o);
			else if (o is double)
				packer.Pack ((double)o);
			else if (o is long)
				packer.Pack ((long)o);
			else if (o is ulong)
				packer.Pack ((ulong)o);
			else if (o is bool)
				packer.Pack ((bool)o);
			else if (o is byte)
				packer.Pack ((byte)o);
			else if (o is sbyte)
				packer.Pack ((sbyte)o);
			else if (o is short)
				packer.Pack ((short)o);
			else if (o is ushort)
				packer.Pack ((ushort)o);
			else if (o is string)
				packer.PackString ((string)o, Encoding.ASCII);
			else if (o is Dictionary<string, object>) {
				packer.PackMapHeader ((o as Dictionary<string, object>).Count);
				foreach (var pair in (o as Dictionary<string, object>)) {
					Pack (packer, pair.Key);
					Pack (packer, pair.Value);
				}
				
			} else if (o is string[]) {
				packer.PackArrayHeader ((o as string[]).Length);
				foreach (var obj in (o as string[]))
					packer.Pack (obj as string);
			} else
				throw new Exception ("Cant handle type: " + o.GetType ().Name);
		}

		public void Dispose ()
		{
			if (this.Token != null){
				this.Execute ("auth.logout", new object[] { });
				_token = null;
			}
		}
	}
}

