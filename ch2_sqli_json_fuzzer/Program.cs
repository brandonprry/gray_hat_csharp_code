using System;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Reflection;

namespace fuzzer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string url = args [0];
			string requestFile = args [1];
			string[] request = null;

			using (StreamReader rdr = new StreamReader (File.OpenRead(requestFile)))
				request = rdr.ReadToEnd ().Split('\n');

			string json = request [request.Length - 1];
			JObject obj = JObject.Parse (json);

			IterateAndFuzz(url, obj);
		}

		private static void IterateAndFuzz (string url, JObject obj)
		{
			foreach (var pair in (JObject)obj.DeepClone()) {
				if (pair.Value.Type == JTokenType.String || pair.Value.Type == JTokenType.Integer) {
					Console.WriteLine("Fuzzing key: " + pair.Key);

					if (pair.Value.Type == JTokenType.Integer)
						Console.WriteLine ("Converting int type to string to fuzz");

					string oldVal = (string)pair.Value;
					obj[pair.Key] = "fd'sa";

					if (Fuzz(url, obj.Root))
						Console.WriteLine("SQL injection vector: " + pair.Key);

					obj[pair.Key] = oldVal;
				}
				else if (pair.Value.Type == JTokenType.Object) {
					IterateAndFuzz(url, (JObject)pair.Value);
				}
			}
		}

		private static bool Fuzz(string url, JToken obj) {
			byte[] data = System.Text.Encoding.ASCII.GetBytes (obj.ToString ());

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
			req.Method = "POST";
			req.ContentLength = data.Length;
			req.ContentType = "application/javascript";

			req.GetRequestStream ().Write (data, 0, data.Length);

			string resp = string.Empty;
			try {
				req.GetResponse ();
			} catch (WebException ex) {
				using (StreamReader rdr = new StreamReader(ex.Response.GetResponseStream()))
					resp = rdr.ReadToEnd ();

				return (resp.Contains ("syntax error") || resp.Contains("unterminated quoted string"));
			}

			return false;
		}
	}
}