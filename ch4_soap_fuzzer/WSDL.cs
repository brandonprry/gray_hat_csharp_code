using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace fuzzer
{
	public class WSDL
	{
		XmlNamespaceManager _nsManager;
		public WSDL (XmlDocument doc)
		{
			_nsManager = new XmlNamespaceManager (doc.NameTable);
			_nsManager.AddNamespace ("wsdl", doc.DocumentElement.NamespaceURI);
			_nsManager.AddNamespace ("xs", "http://www.w3.org/2001/XMLSchema");

			ParseTypes (doc);
			ParseMessages (doc);
			ParsePortTypes (doc);
			ParseBindings (doc);
			ParseServices (doc);
		}

		public List<SoapType> Types { get; set; }

		public List<SoapMessage> Messages { get; set; }

		public List<SoapPortType> PortTypes { get; set; }

		public List<SoapBinding> Bindings { get; set; }

		public List<SoapService> Services { get; set; }


		private void ParseTypes (XmlDocument wsdl)
		{
			this.Types = new List<SoapType> ();
			foreach (XmlNode type in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:types/xs:schema/xs:element", _nsManager))
				this.Types.Add (new SoapType (type));
		}

		private void ParseMessages (XmlDocument wsdl)
		{
			this.Messages = new List<SoapMessage> ();
			foreach (XmlNode node in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:message", _nsManager))
				this.Messages.Add (new SoapMessage (node));
		}

		private void ParsePortTypes (XmlDocument wsdl)
		{
			this.PortTypes = new List<SoapPortType> ();
			foreach (XmlNode node in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:portType", _nsManager)) 
				this.PortTypes.Add (new SoapPortType (node));
		}

		private void ParseBindings (XmlDocument wsdl)
		{
			this.Bindings = new List<SoapBinding> ();
			foreach (XmlNode node in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:binding", _nsManager)) 
				this.Bindings.Add (new SoapBinding (node));
		}

		private void ParseServices (XmlDocument wsdl)
		{
			this.Services = new List<SoapService> ();
			foreach (XmlNode node in wsdl.DocumentElement.SelectNodes("/wsdl:definitions/wsdl:service", _nsManager)) 
				this.Services.Add (new SoapService (node));
		}
	}
}

