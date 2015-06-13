using System;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Reflection;

namespace ch3_soap_fuzzer
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

			Console.WriteLine ("Fuzzing POST requests to URL " + url);
			IterateAndFuzz(url, obj);
		}

		private static void IterateAndFuzz (string url, JObject obj)
		{
			foreach (var pair in (JObject)obj.DeepClone()) {
				if (pair.Value.Type == JTokenType.String || pair.Value.Type == JTokenType.Integer) {
					Console.WriteLine("Fuzzing key: " + pair.Key);

					if (pair.Value.Type == JTokenType.Integer)
						Console.WriteLine ("Converting int type to string to fuzz");

					JToken oldVal = pair.Value;
					obj[pair.Key] = pair.Value.ToString() + "'";

					if (Fuzz (url, obj.Root))
						Console.WriteLine ("SQL injection vector: " + pair.Key);
					else
						Console.WriteLine (pair.Key + " does not seem vulnerable.");

					obj[pair.Key] = oldVal;
				}
			}
		}

		private static bool Fuzz(string url, JToken obj) {
			byte[] data = System.Text.Encoding.ASCII.GetBytes (obj.ToString ());

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
			req.Method = "POST";
			req.ContentLength = data.Length;
			req.ContentType = "application/javascript";

			using (Stream stream = req.GetRequestStream ())
				stream.Write (data, 0, data.Length);

			string resp = string.Empty;
			try {
				req.GetResponse ();
			} catch (WebException e) {
				using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
					resp = r.ReadToEnd ();

				return (resp.Contains ("syntax error") || resp.Contains("unterminated quoted string"));
			}

			return false;
		}
	}
}