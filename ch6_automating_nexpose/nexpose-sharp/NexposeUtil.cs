using System;
using System.Collections.Generic;
using System.Xml;
using nexposesharp;

namespace nexposesharp
{
	public class NexposeUtil
	{		
		public static XmlNode GenerateAdHocReportConfig(string templateID, NexposeReportFormat formatType, Dictionary<NexposeReportFilterType, string> filters)
		{
			XmlDocument doc = new XmlDocument();
			
			string format = string.Empty;
			
			switch (formatType)
			{
				case NexposeReportFormat.CSV:
					format = "csv";
					break;
				case NexposeReportFormat.HTML:
					format = "html";
					break;
				case NexposeReportFormat.PDF:
					format = "pdf";
					break;
				case NexposeReportFormat.QualysXML:
					format = "qualys-xml";
					break;
				case NexposeReportFormat.RawXML:
					format = "raw-xml";
					break;
				case NexposeReportFormat.RawXMLv2:
					format = "raw-xml-v2";
					break;
				case NexposeReportFormat.RTF: 
					format = "rtf";
					break;
				case NexposeReportFormat.Text:
					format = "text";
					break;
				case NexposeReportFormat.XML:
					format = "xml";
					break;
				default:
					throw new Exception("unknown report type.");
				
			}
			
			string xml = "<AdhocReportConfig template-id=\"" + templateID + "\" format=\"" + format + "\">";
			
			xml = xml + "<Filters>";
			
			foreach (KeyValuePair<NexposeReportFilterType, string> filter in filters)
			{
				string filterType = string.Empty;
				
				switch (filter.Key)
				{
				case NexposeReportFilterType.Device:
					filterType = "device";
					break;
				case NexposeReportFilterType.Group:
					filterType = "group";
					break;
				case NexposeReportFilterType.Scan:
					filterType = "scan";
					break;
				case NexposeReportFilterType.Site:
					filterType = "site";
					break;
				case NexposeReportFilterType.VulnerabilitySeverity:
					filterType = "vuln-severity";
					break;
				case NexposeReportFilterType.VulnerabilityCategories:
					filterType = "vuln-categories";
					break;
				default:
					throw new Exception("unknown format type.");
				}
				
				xml = xml + "<filter type=\"" + filterType + "\" id=\"" + filter.Value + "\" />";
			}
			
			xml = xml + "</Filters>";
			
			xml = xml + "</AdhocReportConfig>";
			
			doc.LoadXml(xml);
			
			return doc.FirstChild;
		}
	}
}

