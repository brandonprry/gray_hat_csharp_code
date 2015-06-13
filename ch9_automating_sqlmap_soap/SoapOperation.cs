using System;
using System.Collections.Generic;
using System.Xml;

namespace ch3_soap_fuzzer
{
	public class SoapOperation
	{
		public SoapOperation (XmlNode op)
		{
			this.Name = op.Attributes ["name"].Value;

			foreach (XmlNode message in op.ChildNodes) {
				if (message.Name.EndsWith("input"))
					this.Input = message.Attributes["message"].Value;
				else if (message.Name.EndsWith("output"))
					this.Output = message.Attributes["message"].Value;
				else 
					throw new Exception("Don't know element: " + message.Name);
			}
		}

		public string Name { get; set; }

		public string Input { get; set; }

		public string Output { get; set; }
	}


}

