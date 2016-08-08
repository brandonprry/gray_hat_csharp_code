using System;
using System.Xml;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ch6_automating_nexpose
{
	public class NexposeSession : IDisposable
	{
		public NexposeSession(string username, string password, string host, int port = 3780, NexposeAPIVersion version = NexposeAPIVersion.v11)
		{
			this.Host = host;
			this.Port = port;
			this.APIVersion = version;

			this.Authenticate(username, password);
		}

		public string Host { get; set; }

		public int Port { get; set; }

		public bool IsAuthenticated { get; set; }

		public string SessionID { get; set; }

		public NexposeAPIVersion APIVersion { get; set; }

		public XDocument Authenticate (string username, string password)
		{
			XDocument cmd = new XDocument (
				                new XElement ("LoginRequest",
					                new XAttribute ("user-id", username),
					                new XAttribute ("password", password)));

			XDocument doc = (XDocument)this.ExecuteCommand (cmd);

			if (doc.Root.Attribute ("success").Value == "1") {
				this.SessionID = doc.Root.Attribute ("session-id").Value;
				this.IsAuthenticated = true;
			} else
				throw new Exception ("Authentication failed");
			
			return doc;
		}

		public object ExecuteCommand (XDocument commandXml)
		{
			string uri = string.Empty;
			switch (this.APIVersion) {
			case NexposeAPIVersion.v11:
				uri = "/api/1.1/xml";
				break;
			case NexposeAPIVersion.v12:
				uri = "/api/1.2/xml";
				break;
			default:
				throw new Exception ("Unknown API version.");
			}
			byte[] byteArray = Encoding.ASCII.GetBytes (commandXml.ToString ());
			HttpWebRequest request = WebRequest.Create ("https://" + this.Host + ":" + this.Port + uri) as HttpWebRequest;
			request.Proxy = new WebProxy("127.0.0.1:8080");
			request.Method = "POST";
			request.ContentType = "text/xml";
			request.ContentLength = byteArray.Length;

			using (Stream dataStream = request.GetRequestStream ())
				dataStream.Write (byteArray, 0, byteArray.Length);

			string xml = string.Empty;

			using (HttpWebResponse r = request.GetResponse () as HttpWebResponse) {
				using (StreamReader reader = new StreamReader (r.GetResponseStream ()))
					xml = reader.ReadToEnd ();

				if (r.ContentType.Contains("multipart/mixed")) {
					string[] splitRequest = xml
						.Split (new string[] {"--AxB9sl3299asdjvbA"}, StringSplitOptions.None);
					
					splitRequest = splitRequest [2]
						.Split (new string[] { "\r\n\r\n" }, StringSplitOptions.None);

					string base64Data = splitRequest [1]
						.Substring (0, splitRequest [1].IndexOf ("DQo="));
					
					return Convert.FromBase64String (base64Data);
				}
			}

			return XDocument.Parse (xml);
		}

		public XDocument Logout ()
		{
			XDocument cmd = new XDocument (
				                new XElement ("LogoutRequest",
					                new XAttribute ("session-id", this.SessionID)));

			XDocument doc = (XDocument)this.ExecuteCommand (cmd);

			this.IsAuthenticated = false;
			this.SessionID = string.Empty;
			 
			return doc;
		}

		public void Dispose ()
		{
			if (this.IsAuthenticated)
				this.Logout ();
		}
	}
}

