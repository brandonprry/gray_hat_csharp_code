using System;
using System.Collections.Generic;
using System.Xml;

namespace ch9_soap_fuzzer
{
	public class SoapPortType
	{
		public SoapPortType (XmlNode node)
		{
			this.Name = node.Attributes ["name"].Value;
			this.Operations = new List<SoapOperation>();
			foreach (XmlNode op in node.ChildNodes) {
				this.Operations.Add(new SoapOperation(op));
			}
		}

		public string Name { get; set; } 

		public List<SoapOperation> Operations { get; set; }

	}

}

