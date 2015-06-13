using System;
using System.Xml;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace nexposesharp
{
	public class NexposeSession : IDisposable
	{
		public NexposeSession (string host, int port = 3780, NexposeAPIVersion version = NexposeAPIVersion.v11)
		{
			this.Host = host;
			this.Port = port;
			this.APIVersion = version;
		}

		public string Host { get; set; }
		
		public int Port { get; set; }
		
		public bool IsAuthenticated { get; set; }
		
		public string SessionID { get; set; }
		
		public NexposeAPIVersion APIVersion { get; set; }
		
		public XDocument Authenticate(string username, string password)
		{
			XDocument cmd = new XDocument("LoginRequest",
				new XAttribute("user-id", username),
				new XAttribute("password", password));

			XDocument doc = this.ExecuteCommand(cmd);		

			this.IsAuthenticated = true;
			
			return doc;
		}
		
		public XDocument Logout()
		{
			XDocument cmd = new XDocument("LogoutRequest",
				new XAttribute("session-id", this.SessionID));

			XDocument doc = this.ExecuteCommand(cmd);

			this.IsAuthenticated = false;
			this.SessionID = string.Empty;
			 
			return doc;
		}

		public XDocument ExecuteCommand(XDocument commandXml)
		{
			string uri = string.Empty;
			
		    switch (this.APIVersion)
			{
				case NexposeAPIVersion.v11:
					uri = "/api/1.1/xml";
					break;
				case NexposeAPIVersion.v12:
					uri = "/api/1.2/xml";
					break;
				default:
					throw new Exception("unknown api version.");
			}
			
			HttpWebRequest request = WebRequest.Create("https://" + this.Host + ":" + this.Port.ToString() + uri) as HttpWebRequest;
			
			ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true; //anonymous lamda expressions ftw!
			
			request.KeepAlive = true;
			request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
			request.ContentType = "text/xml";
			
			byte[] byteArray = Encoding.ASCII.GetBytes(commandXml.ToString());
            
            request.ContentLength = byteArray.Length;
			
            using (Stream dataStream = request.GetRequestStream())
            	dataStream.Write(byteArray, 0, byteArray.Length);
			
			string xml = string.Empty;
            using (HttpWebResponse r = request.GetResponse() as HttpWebResponse)
                using (StreamReader reader = new StreamReader(r.GetResponseStream()))
					xml = reader.ReadToEnd();
		
			return XDocument.Parse (xml);
		}
	
		public void Dispose()
		{
			if (this.IsAuthenticated)
				this.Logout();
		}
	}
}

