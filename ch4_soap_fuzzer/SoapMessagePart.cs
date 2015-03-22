using System;
using System.Collections.Generic;
using System.Xml;

namespace fuzzer
{
	public class SoapMessagePart
	{
		public SoapMessagePart (XmlNode part)
		{
			this.Name = part.Attributes["name"].Value;

			if (part.Attributes["element"] != null)
				this.Element = part.Attributes["element"].Value;
			else 
				this.Type = part.Attributes["type"].Value;
		}

		public string Name { get; set; }

		public string Element { get; set; }

		public string Type  { get; set; }
	}
}

