using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Xml;
using System.Text.RegularExpressions;

namespace openvassharp
{
	public class OpenVASManagerSession : IDisposable
	{
		private static bool? _careAboutSSL;
		private SslStream _stream;
		
		public OpenVASManagerSession (string username, string password, string host)
		{
			this.Initiate (host, "OpenVAS");
			this.Authenticate (username, password);
		}
		
		public string Username { get; set; }
		
		public string Password { get; set; }
		
		public IPAddress ServerIPAddress { get; set; }

		public string ServerName { get; private set; }

		public SslStream Stream { 
			get {
				if (_stream == null || !_stream.IsAuthenticated)
					GetStream ();
				
				return _stream;
			}
			
			set {
				_stream = value;	
			}
				
		}

		public bool IsAuthenticated { get; private set; }
		
		public bool? CareAboutSSL {
			get {
				if (_careAboutSSL == null)
					return true;
				
				return _careAboutSSL;
			}
			set {
				_careAboutSSL = value;
			}
		}
		
		public void Initiate ()
		{
			if (this.ServerIPAddress == null)
				throw new Exception ("Need an IP address");
			
			GetStream ();
		}

		public void Initiate (string ipaddress, string servName)
		{
			if (string.IsNullOrEmpty (ipaddress))
				throw new Exception ("Invalid IP address");
			
			if (string.IsNullOrEmpty (servName))
				throw new Exception ("Invalid server name");
			
			this.ServerIPAddress = IPAddress.Parse (ipaddress);
			this.ServerName = servName;
			
			GetStream ();
		}

		public void Initiate (IPAddress ipaddress, string servName)
		{
			if (ipaddress == null)
				throw new ArgumentNullException ("IP address null");
			
			if (string.IsNullOrEmpty (servName))
				throw new Exception ("Invalid server name: " + servName);
			
			this.ServerIPAddress = ipaddress;
			this.ServerName = servName;
			
			GetStream ();
		}

		public XmlDocument Authenticate (string username, string password)
		{
			ASCIIEncoding enc = new ASCIIEncoding ();
			
			if (this.Stream == null || !this.Stream.CanRead)
				throw new Exception ("Stream null or otherwise broken");
			
			this.Username = username;
			this.Password = password;
			
			string xml = "<authenticate><credentials><username>" + username + "</username><password>" + password + "</password></credentials></authenticate>";
			
			this.Stream.Write (enc.GetBytes (xml));
			
			
			string response = ReadMessage (this.Stream);
			
			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (response);
			
			if (doc.DocumentElement.LocalName == "authenticate_response") {
				foreach (XmlAttribute attr in doc.DocumentElement.Attributes) {
					if (attr.Name == "status") {
						if (attr.Value == "200") {
							this.IsAuthenticated = true;
							break;
						} else
							throw new Exception ("Bad username/password.");
					}
				}
			}
			
			this.Stream.Flush ();
			
			this.Stream = null;
			
			return doc;		
		}

		public XmlDocument ExecuteCommand (XmlDocument doc)
		{
			if (!this.IsAuthenticated)
				throw new Exception ("Not authenticated");
			
			this.Initiate ();
			
			ASCIIEncoding enc = new ASCIIEncoding ();
			string auth = "<authenticate><credentials><username>" + this.Username + "</username><password>" + this.Password + "</password></credentials></authenticate>";
			string xml = doc.OuterXml;
			
			string response = string.Empty;
			try {
				this.Stream.Write (enc.GetBytes (auth + xml), 0, auth.Length + xml.Length);
				
				response = ReadMessage (this.Stream)
					.Replace ("<authenticate_response status=\"200\" status_text=\"OK\"/>", string.Empty);
				
				string[] tmp = Regex.Split (response, "</authenticate_response>");
				
				
				XmlDocument docResponse = new XmlDocument ();
				
				if (tmp.Length > 1)
					docResponse.LoadXml (tmp [1]);
				else
					docResponse.LoadXml (response);
			
				this.Stream = null;
				
				return docResponse;
			} catch (Exception ex) {
				this.Stream = null;
				
				throw ex;
			}
		}

		private void GetStream ()
		{
			if (_stream == null || !_stream.CanRead) {
				TcpClient c = new TcpClient (ServerIPAddress.ToString (), 9390);
				SslStream s;
				
				try {
					s = new SslStream (c.GetStream (), false, new RemoteCertificateValidationCallback (ValidateServerCertificate), 
						(sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) => null);
					
					s.AuthenticateAsClient ("openvas", null, System.Security.Authentication.SslProtocols.Tls, false);
					
				} catch (Exception ex) {
					throw ex;
				}
				
				_stream = s;
			}
		}

		private string ReadMessage (SslStream sslStream)
		{

			// Read the  message sent by the server. The end of the message is signaled using the "<EOF>" marker.
			byte[ ] buffer = new byte[2048];
			StringBuilder messageData = new StringBuilder ();
			int bytes = -1;
			do {
				bytes = sslStream.Read (buffer, 0, buffer.Length);
				
				// Use Decoder class to convert from bytes to UTF8 in case a character spans two buffers.
				Decoder decoder = Encoding.UTF8.GetDecoder ();
				char[ ] chars = new char[decoder.GetCharCount (buffer, 0, bytes)];
				decoder.GetChars (buffer, 0, bytes, chars, 0);
				messageData.Append (chars);
				
				// Check for EOF.
				if (bytes < buffer.Length) {
					bytes = 0;
					return messageData.ToString ();
				}
				
				buffer = new byte[2048]; //clear cruft
			} while (bytes != 0);
			
			return messageData.ToString ();
		}

		private static bool ValidateServerCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (!_careAboutSSL.HasValue || !_careAboutSSL.Value)
				return true;
			
			if (sslPolicyErrors == SslPolicyErrors.None)
				return true;
			
			Console.WriteLine ("Certificate error: {0}", sslPolicyErrors);
			
			return false;
		}
		
		public void Dispose ()
		{
			
		}
		
	}
}

