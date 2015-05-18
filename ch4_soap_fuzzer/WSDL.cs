using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace fuzzer
{
	public class WSDL
	{
		public WSDL (XmlDocument doc)
		{
			XmlNamespaceManager nsManager = new XmlNamespaceManager (doc.NameTable);
			nsManager.AddNamespace ("wsdl", doc.DocumentElement.NamespaceURI);
			nsManager.AddNamespace ("xs", "http://www.w3.org/2001/XMLSchema");

			ParseTypes (doc, nsManager);
			ParseMessages (doc, nsManager);
			ParsePortTypes (doc, nsManager);
			ParseBindings (doc, nsManager);
			ParseServices (doc, nsManager);
		}

		public List<SoapType> Types { get; set; }
		public List<SoapMessage> Messages { get; set; }
		public List<SoapPortType> PortTypes { get; set; }
		public List<SoapBinding> Bindings { get; set; }
		public List<SoapService> Services { get; set; }

		private void ParseTypes (XmlDocument wsdl, XmlNamespaceManager nsManager)
		{
			this.Types = new List<SoapType> ();
			string xpath = "/wsdl:definitions/wsdl:types/xs:schema/xs:element";
			XmlNodeList nodes = wsdl.DocumentElement.SelectNodes(xpath, nsManager);
			foreach (XmlNode type in nodes)
				this.Types.Add (new SoapType (type));
		}

		private void ParseMessages (XmlDocument wsdl, XmlNamespaceManager nsManager)
		{
			this.Messages = new List<SoapMessage> ();
			string xpath = "/wsdl:definitions/wsdl:message";
			XmlNodeList nodes = wsdl.DocumentElement.SelectNodes(xpath, nsManager);
			foreach (XmlNode node in nodes)
				this.Messages.Add (new SoapMessage (node));
		}

		private void ParsePortTypes (XmlDocument wsdl, XmlNamespaceManager nsManager)
		{
			this.PortTypes = new List<SoapPortType> ();
			string xpath = "/wsdl:definitions/wsdl:portType";
			XmlNodeList nodes = wsdl.DocumentElement.SelectNodes(xpath, nsManager);
			foreach (XmlNode node in nodes) 
				this.PortTypes.Add (new SoapPortType (node));
		}

		private void ParseBindings (XmlDocument wsdl, XmlNamespaceManager nsManager)
		{
			this.Bindings = new List<SoapBinding> ();
			string xpath = "/wsdl:definitions/wsdl:binding";
			XmlNodeList nodes = wsdl.DocumentElement.SelectNodes(xpath, nsManager);
			foreach (XmlNode node in nodes) 
				this.Bindings.Add (new SoapBinding (node));
		}

		private void ParseServices (XmlDocument wsdl, XmlNamespaceManager nsManager)
		{
			this.Services = new List<SoapService> ();
			string xpath = "/wsdl:definitions/wsdl:service";
			XmlNodeList nodes = wsdl.DocumentElement.SelectNodes(xpath, nsManager);
			foreach (XmlNode node in nodes) 
				this.Services.Add (new SoapService (node));
		}
	}
}

