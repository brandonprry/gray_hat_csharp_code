using System;
using System.Linq;
using System.Xml;

namespace ch3_soap_fuzzer
{
	public class SoapBindingOperation
	{
		public SoapBindingOperation (XmlNode op)
		{
			this.Name = op.Attributes ["name"].Value;

			foreach (XmlNode node in op.ChildNodes) {
				if (node.Name == "http:operation")
					this.Location = node.Attributes["location"].Value;
				if (node.Name == "soap:operation" || node.Name == "soap12:operation")
					this.SoapAction = node.Attributes["soapAction"].Value;
			}
		}

		public string Name { get; set; }
		public string Location { get; set; }
		public string SoapAction { get; set; }
	}
}

