using System;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace ch6_automating_nessus
{
	public class NessusSession : IDisposable
	{
		public NessusSession (string host, string username, string password){
			this.Host = host;

			if (!Authenticate (username, password))
				throw new Exception ("Authentication failed");
		}

		public bool Authenticate(string username, string password) {
			JObject obj = new JObject ();
			obj ["username"] = username;
			obj ["password"] = password;

			JObject ret = MakeRequest("POST", "/session", obj);

			if (ret ["token"] == null)
				return false;

			this.Token = ret ["token"].Value<string> ();
			this.Authenticated = true;

			return true;
		}

		public void LogOut(){
			if (this.Authenticated) {
				MakeRequest ("DELETE", "/session", null, this.Token);
				this.Authenticated = false;
			}
		}


		public void Dispose() {
			if (this.Authenticated)
				this.LogOut ();
		}

		public JObject MakeRequest(string verb, string uri, JObject data = null, string token = null){
			string url = "https://" + this.Host + ":8834" + uri;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			request.Method = verb;
			//request.Proxy = new WebProxy ("127.0.0.1", 8080);

			if (!string.IsNullOrEmpty (token))
				request.Headers ["X-Cookie"] = "token=" + token;

			request.ContentType = "application/json";

			if (data != null) {
				byte[] bytes = System.Text.Encoding.ASCII.GetBytes (data.ToString ());
				request.ContentLength = bytes.Length;
				request.GetRequestStream ().Write (bytes, 0, bytes.Length);
			} else
				request.ContentLength = 0;

			string response = string.Empty;
			using (StreamReader reader = new StreamReader (request.GetResponse ().GetResponseStream ()))
				response = reader.ReadToEnd ();

			if (string.IsNullOrEmpty (response))
				return new JObject ();

			return JObject.Parse (response);
		}

		public string Host { get; set; }
		public bool Authenticated { get; private set; }
		public string Token { get; private set; }
	}
}

