using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading;
using System.IO;
using System.Net;

namespace ch6_automating_nexpose
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

			using (NexposeSession session = new NexposeSession ("nxadmin", "nxpassword", "172.18.20.37")) {
				using (NexposeManager manager =	new NexposeManager (session)) {
					
					Console.WriteLine (manager.GetSystemInformation ().ToString ());

					string[][] ips = {
						new string[] { "192.168.2.169", string.Empty }
					};
						
					XDocument site = manager.CreateOrUpdateSite (Guid.NewGuid ().ToString (), null, ips);

					int siteID = int.Parse (site.Root.Attribute ("site-id").Value);

					XDocument scan = manager.ScanSite (siteID);

					XElement ele = scan.XPathSelectElement ("//SiteScanResponse/Scan");

					int scanID = int.Parse(ele.Attribute ("scan-id").Value);

					XDocument status = manager.GetScanStatus (scanID);

					while (status.Root.Attribute ("status").Value != "finished") {
						Thread.Sleep (1000);
						status = manager.GetScanStatus (scanID);
						Console.WriteLine (DateTime.Now.ToLongTimeString() + ": " + status.ToString ());
					}

					byte[] report = manager.GetPdfSiteReport (siteID);

					File.WriteAllBytes ("/tmp/fdsa.pdf", report);

					manager.DeleteSite (siteID);
				}
			}
		}
	}
}
