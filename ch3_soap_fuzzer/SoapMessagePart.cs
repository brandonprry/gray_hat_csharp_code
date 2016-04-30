using System;
using System.Collections.Generic;
using System.Xml;

namespace ch3_soap_fuzzer
{
	public class SoapMessagePart
	{
		public SoapMessagePart (XmlNode part)
		{
			this.Name = part.Attributes["name"].Value;

			if (part.Attributes["element"] != null)
				this.Element = part.Attributes["element"].Value;
			else if (part.Attributes["type"] != null)
				this.Type = part.Attributes["type"].Value;
			else
				throw new ArgumentException("Neither element nor type attribute exist", nameof(part));
		}

		public string Name { get; set; }
		public string Element { get; set; }
		public string Type  { get; set; }
	}
}

