using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace ch14_automating_arachni
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (ArachniHTTPSession session = new ArachniHTTPSession ("127.0.0.1", 7331)) {
				using (ArachniHTTPManager manager = new ArachniHTTPManager (session)) {

					JObject scanOptions = new JObject ();
					scanOptions ["checks"] = new JArray() { "xss*", "sql*" } ;
					scanOptions ["audit"] = new JObject ();
					scanOptions ["audit"]["elements"] = new JArray() { "links", "forms" };

					JObject scanId = manager.StartScan ("http://demo.testfire.net/default.aspx", scanOptions);
					Guid id = Guid.Parse(scanId ["id"].ToString ());
					JObject scan = manager.GetScanStatus (id);

					while (scan ["status"].ToString() != "done") {
						Console.WriteLine ("Sleeping");
						System.Threading.Thread.Sleep (100);
						scan = manager.GetScanStatus (id);
					}

					Console.WriteLine (scan.ToString ());
				}
			}
		}
	}
}
