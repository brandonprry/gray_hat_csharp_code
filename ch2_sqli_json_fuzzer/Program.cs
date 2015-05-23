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
			Assembly asm = Assembly.GetExecutingAssembly ();

			string json = string.Empty;
			using (StreamReader rdr = new StreamReader(asm.GetManifestResourceStream("fuzzer.CreateUser.json")))
				json = rdr.ReadToEnd ();

			JObject obj = JObject.Parse (json);

			IterateAndFuzz(args[0], obj);
		}

		private static void IterateAndFuzz (string url, JObject obj)
		{
			foreach (var pair in (JObject)obj.DeepClone()) {
				if (pair.Value.Type != JTokenType.String && pair.Value.Type != JTokenType.Object) {
					Console.WriteLine("Skipping JSON key: " + pair.Key);
					continue;
				}

				if (pair.Value.Type == JTokenType.String) {
					Console.WriteLine("Fuzzing key: " + pair.Key);
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
			byte[] data = System.Text.Encoding.ASCII.GetBytes ("JSON=" + obj.ToString ());

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
			req.Method = "POST";
			req.ContentLength = data.Length;
			req.ContentType = "application/x-www-form-urlencoded";

			req.GetRequestStream ().Write (data, 0, data.Length);

			string resp = string.Empty;
			try {
				req.GetResponse ();
			} catch (WebException ex) {
				using (StreamReader rdr = new StreamReader(ex.Response.GetResponseStream()))
					resp = rdr.ReadToEnd ();

				return resp.Contains ("syntax error");
			}

			return false;
		}
	}
}