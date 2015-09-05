using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading;
using System.IO;

namespace ch6_automating_nexpose
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (NexposeSession session = new NexposeSession ("admin", "Passw0rd!", "192.168.2.171")) {
				using (NexposeManager manager =	new NexposeManager (session)) {
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
