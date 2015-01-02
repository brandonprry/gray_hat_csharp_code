using System;
using System.Collections.Generic;
using System.Xml;

namespace fuzzer
{
	public class SoapMessage
	{
		public SoapMessage (XmlNode node)
		{
			this.Name = node.Attributes ["name"].Value;
			this.Parts = new List<SoapPart> ();

			if (node.HasChildNodes) {
				foreach (XmlNode part in node.ChildNodes)
					this.Parts.Add(new SoapPart(part));
			}
		}

		public string Name { get; set; }

		public List<SoapPart> Parts { get; set; }
	}

}

