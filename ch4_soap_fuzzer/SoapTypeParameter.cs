using System;
using System.Xml;

namespace ch3_soap_fuzzer
{
	public class SoapTypeParameter
	{
		public SoapTypeParameter (XmlNode node)
		{
			if (node.Attributes["maxOccurs"].Value == "unbounded")
				this.MaximumOccurence = int.MaxValue;
			else
				this.MaximumOccurence = int.Parse(node.Attributes["maxOccurs"].Value);

			this.MinimumOccurence = int.Parse(node.Attributes["minOccurs"].Value);
			this.Name = node.Attributes["name"].Value;
			this.Type = node.Attributes["type"].Value;
		}

		public int MinimumOccurence { get; set; }
		public int MaximumOccurence { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
	}
}

