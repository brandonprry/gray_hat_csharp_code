using System;
using System.Collections.Generic;
using System.Xml;

namespace ch9_soap_fuzzer
{
	public class SoapPort
	{
		public SoapPort (XmlNode port)
		{
			this.Name = port.Attributes["name"].Value;
			this.Binding = port.Attributes["binding"].Value;
			this.ElementType = port.FirstChild.Name;
			this.Location = port.FirstChild.Attributes["location"].Value;
		}

		public string Name { get; set; }

		public string Binding { get; set; }

		public string ElementType { get; set; }

		public string Location { get; set; }
	}


}