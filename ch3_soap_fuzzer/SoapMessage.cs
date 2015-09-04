using System;
using System.Collections.Generic;
using System.Xml;

namespace ch3_soap_fuzzer
{
	public class SoapMessage
	{
		public SoapMessage (XmlNode node)
		{
			this.Name = node.Attributes ["name"].Value;
			this.Parts = new List<SoapMessagePart> ();

			if (node.HasChildNodes) {
				foreach (XmlNode part in node.ChildNodes)
					this.Parts.Add(new SoapMessagePart(part));
			}
		}

		public string Name { get; set; }

		public List<SoapMessagePart> Parts { get; set; }
	}

}

