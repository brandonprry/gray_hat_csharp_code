using System;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ch6_automating_nexpose
{
	public class NexposeManager : IDisposable
	{
		private NexposeSession _session;
		
		public NexposeManager (NexposeSession session)
		{
			if (!session.IsAuthenticated)
				throw new Exception("Trying to create manager from unauthenticated session. Please authenticate.");
			
			_session = session;
		}

		public XDocument CreateOrUpdateSite(string name, string[] hostnames = null, string[][] ips = null, int siteID = -1) {

			XElement hosts = new XElement ("Hosts");

			if (hostnames != null) {
				foreach (string host in hostnames)
					hosts.Add (new XElement ("host", host));
			}

			if (ips != null) {
				foreach (string[] range in ips) {
					hosts.Add (new XElement ("range",
						new XAttribute ("from", range [0]),
						new XAttribute ("to", range [1])));
				}
			}

			XDocument xml = new XDocument (
				                new XElement ("SiteSaveRequest",
					                new XAttribute ("session-id", _session.SessionID),
					                new XElement ("Site",
						                new XAttribute ("id", siteID),
						                new XAttribute ("name", name),
						                hosts,
						                new XElement ("ScanConfig",
							                new XAttribute ("name", "Full audit"),
							                new XAttribute ("templateID", "full-audit")))));

			return (XDocument)_session.ExecuteCommand (xml);
		}

		public XDocument ScanSite(int siteID) {

			XDocument xml = new XDocument (
				                new XElement ("SiteScanRequest",
					                new XAttribute ("session-id", _session.SessionID),
					                new XAttribute ("site-id", siteID)));
			return (XDocument)_session.ExecuteCommand (xml);
		}

		public XDocument GetScanStatus(int scanID) {
			XDocument xml = new XDocument (
				                new XElement ("ScanStatusRequest",
					                new XAttribute ("session-id", _session.SessionID),
					                new XAttribute ("scan-id", scanID)));
			
			return (XDocument)_session.ExecuteCommand (xml);
		}

		public byte[] GetPdfSiteReport(int siteID) {
			XDocument doc = new XDocument (
				new XElement("ReportAdhocGenerateRequest",
					new XAttribute("session-id", _session.SessionID),
					new XElement ("AdhocReportConfig",
						new XAttribute ("template-id", "audit-report"),
						new XAttribute ("format", "pdf"),
						new XElement ("Filters", 
							new XElement("filter", 
								new XAttribute("type", "site"),
								new XAttribute("id", siteID))))));

			return (byte[])_session.ExecuteCommand(doc);
		}

		public XDocument DeleteSite(int siteID) {
			return null;
		}

		public XDocument GetSystemInformation(){
			XDocument xml = new XDocument (
				new XElement ("SystemInformationRequest",
					new XAttribute ("session-id", _session.SessionID)));
		
			return (XDocument)_session.ExecuteCommand (xml);
		}

		public void Dispose()
		{
			_session.Logout();
		}
	}
}

