using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace ch3_soap_fuzzer
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
			foreach (XmlNode type in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:types/xs:schema/xs:element", nsManager))
				this.Types.Add (new SoapType (type));
		}

		private void ParseMessages (XmlDocument wsdl, XmlNamespaceManager nsManager)
		{
			this.Messages = new List<SoapMessage> ();
			foreach (XmlNode node in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:message", nsManager))
				this.Messages.Add (new SoapMessage (node));
		}

		private void ParsePortTypes (XmlDocument wsdl, XmlNamespaceManager nsManager)
		{
			this.PortTypes = new List<SoapPortType> ();
			foreach (XmlNode node in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:portType", nsManager)) 
				this.PortTypes.Add (new SoapPortType (node));
		}

		private void ParseBindings (XmlDocument wsdl, XmlNamespaceManager nsManager)
		{
			this.Bindings = new List<SoapBinding> ();
			foreach (XmlNode node in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:binding", nsManager)) 
				this.Bindings.Add (new SoapBinding (node));
		}

		private void ParseServices (XmlDocument wsdl, XmlNamespaceManager nsManager)
		{
			this.Services = new List<SoapService> ();
			foreach (XmlNode node in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:service", nsManager)) 
				this.Services.Add (new SoapService (node));
		}
	}
}

