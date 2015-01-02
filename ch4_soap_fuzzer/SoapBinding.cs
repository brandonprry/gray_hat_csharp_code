using System;
using System.Collections.Generic;
using System.Xml;

namespace fuzzer
{
	public class SoapBinding
	{
		public SoapBinding (XmlNode node)
		{
			this.Name = node.Attributes ["name"].Value;
			this.Type = node.Attributes ["type"].Value;
			this.IsHTTP = false;
			this.Operations = new List<SoapBindingOperation>();
			foreach (XmlNode op in node.ChildNodes) {
				if (op.Name.EndsWith("operation")) {
					this.Operations.Add(new SoapBindingOperation(op));
				}
				else if (op.Name == "http:binding") {
					this.Verb = op.Attributes["verb"].Value;
					this.IsHTTP = true;
				}
				else if (op.Name == "soap:binding" || op.Name == "soap12:binding") {
					//do nothing really
				}
			}
		}

		public string Name { get; set; }
		public List<SoapBindingOperation> Operations { get; set; }
		public bool IsHTTP { get; set; }
		public string Verb { get; set; }
		public string Type { get; set; }
	}

}

