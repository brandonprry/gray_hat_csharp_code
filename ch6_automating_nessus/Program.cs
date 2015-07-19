using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Newtonsoft.Json.Linq;

namespace ch6_automating_nessus
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ServicePointManager.ServerCertificateValidationCallback = 
				(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true;

			using (NessusSession session = new NessusSession ("192.168.1.14", "admin", "password")) {
				using (NessusManager manager = new NessusManager (session)) {
					JObject policies = manager.GetScanPolicies ();

					string discoveryPolicyID = string.Empty;
					foreach (JObject obj in policies["templates"]){
						if (obj ["name"].Value<string> () == "discovery")
							discoveryPolicyID = obj ["uuid"].Value<string>();
					}

					JObject scan = manager.CreateScan (discoveryPolicyID, "192.168.1.0/24", "Network Scan", "fdsa");

					int scanID = scan ["scan"] ["id"].Value<int> ();

					manager.StartScan (scanID);

					JObject scans = manager.GetScans ();
					Console.WriteLine (scans.ToString ());
				}
			}
		}
	}
}
