using System;
using System.Xml;
using System.Collections.Generic;

namespace nexposesharp
{
	public class ReportConfig
	{
		//sane defaults
		private string _id = "-1";
		private string _trendDateRange = "years_1";
		private string _secondAxis = "averageRisk";
		private string _siteAxis = "aggregateRisk";
		private string _groupAxis = "aggregateRisk";
		private string _compareTo = string.Empty;
		private bool _useAssetGroupMemberships = false;
		private bool _allAssetRiskTrend = true;
		private bool _aggregateAxis = false;
		private bool _siteRiskTrendOptions = false;
		private bool _groupRiskTrendOptions = false; 
		private bool _assetRiskTrendOptions = false;
		
		public ReportConfig ()
		{
		}
		
		public bool GroupRiskTrendOptions { 
			get { return _groupRiskTrendOptions; }
			set { _groupRiskTrendOptions = value; }
		}
		
		public bool AssetRiskTrendOptions { 
			get { return _assetRiskTrendOptions; }
			set { _assetRiskTrendOptions = value; }
		}
		
		public bool SiteRiskTrendOptions {
			get { return _siteRiskTrendOptions; }
			set { _siteRiskTrendOptions = value; }
		}
		
		public bool AllAssetRiskTrend {
			get { return _allAssetRiskTrend; }
			set { _allAssetRiskTrend = value; }
		}
		
		public bool AggregateAxis {
			get { return _aggregateAxis; }
			set { _aggregateAxis = value; }
		}
		
		public string ID { 
			get { return _id; } 
			set { _id = value; }
		}

		public string CompareTo {
			get { return _compareTo; }
			set { _compareTo = value; }
		}
		
		public string TrendDateRange { 
			get { return _trendDateRange; }
			set { _trendDateRange = value; } 
		}
		
		public string SecondAxis {
			get { return _secondAxis; }
			set { _secondAxis = value; }
		}
		
		public string SiteAxis { 
			get { return _siteAxis; }
			set { _siteAxis = value; }
		}
		
		public string GroupAxis { 
			get { return _groupAxis; }
			set { _groupAxis = value; }
		}
		
		public bool UseAssetGroupMemberships {
			get { return _useAssetGroupMemberships; }
			set { _useAssetGroupMemberships = value; }
		}
		
		public string Name { get; set; }
		
		public string TemplateID { get; set; }
		
		public Dictionary<NexposeReportFilterType, string> Filters { get; set; }
		
		public bool StoreOnServer { get; set; }
		
		public NexposeReportFormat Format { get; set; }
		
		public string Owner { get; set; }
		
		public string Timezone { get; set; }
		
		public bool Scheduled { get; set; }
		
		public bool GenerateNow { get; set; }
		
		public string ScheduleType { get; set; }
		
		public DateTime EarliestGenerationTime { get; set; }
		
		public DateTime DisableGenerationAfter { get; set; }
		
		public XmlNode GenerateXML()
		{
			XmlDocument doc = new XmlDocument();
			
			string format = string.Empty;
			
			switch (this.Format)
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
				case NexposeReportFormat.DBExport:
					format = "db";
					break;
				default:
					throw new Exception("unknown report type.");
				
			}
			
			if (string.IsNullOrEmpty(this.Name))
				throw new Exception("Name is empty or null.");
			
			if (string.IsNullOrEmpty(this.TemplateID))
				throw new Exception("TemplateID is null or empty.");
			
			if (this.Filters == null || this.Filters.Count == 0)
				throw new Exception("I need some filters.");
			
			string xml = string.Empty;
			xml += "<ReportConfig id=\"" + this.ID + "\" name=\"" + this.Name + "\" template-id=\"" + this.TemplateID + "\" format=\"" + format + "\">";
			xml += "<Filters>";
			foreach (KeyValuePair<NexposeReportFilterType, string> pair in this.Filters)
			{
				xml += "<filter type=\"";
				
				switch (pair.Key)
				{
				case NexposeReportFilterType.Device:
					xml += "device\"";
					break;
				case NexposeReportFilterType.Group:
					xml += "group\"";
					break;
				case NexposeReportFilterType.Scan:
					xml += "scan\"";
					break;
				case NexposeReportFilterType.Site:
					xml += "site\"";
					break;
				case NexposeReportFilterType.VulnerabilityCategories:
					xml += "vuln-categories\"";
					break;
				case NexposeReportFilterType.VulnerabilitySeverity:
					xml += "vuln-severity\"";
					break;
				case NexposeReportFilterType.VulnerabilityStatus:
					xml += "vuln-status\"";
					break;
				default:
					throw new Exception("I don't know how to interpret this Report Filter.");
				}
				
				xml += " id=\"" + pair.Value + "\" />";
			}
			
			xml += "</Filters>";
			xml += "<RiskTrendConfig useAssetGroupMemberships=\"" + (this.UseAssetGroupMemberships ? "1" : "0") + "\" trendDateRange=\""+ this.TrendDateRange +"\">";
			xml += "<AllAssetRiskTrend enabled=\"" + (this.AllAssetRiskTrend ? "1" : "0") + "\">";
			xml += "<AggregateAxis enabled=\"" + (this.AggregateAxis ? "1" : "0") + "\"/>";
			xml += "<SecondAxis value=\"" + this.SecondAxis + "\"/>";
			xml += "</AllAssetRiskTrend>";
			xml += "<SiteRiskTrendOptions enabled=\"" + (this.SiteRiskTrendOptions ? "1" : "0") + "\">";
			xml += "<SiteAxis value=\"" + this.SiteAxis + "\"/>";
			xml += "</SiteRiskTrendOptions>";
			xml += "<GroupRiskTrendOptions enabled=\"" + (this.GroupRiskTrendOptions ? "1" : "0") + "\">";
			xml += "<GroupAxis value=\"" + this.GroupAxis + "\"/></GroupRiskTrendOptions>";
			xml += "<AssetRiskTrendOptions enabled=\"" + (this.AssetRiskTrendOptions ? "1" : "0") + "\"/>";
			xml += "</RiskTrendConfig>";
			xml += "<Users>";
			xml += "</Users>";

			if (this.CompareTo != string.Empty)
				xml += "<Baseline compareTo=\"" + this.CompareTo + "\" />";

			xml += "<Delivery>";
			xml += "<Storage storeOnServer=\"" + (this.StoreOnServer ? "1" : "0") + "\"><location></location></Storage>";
			xml += "</Delivery>";
			xml += "</ReportConfig>";
			
			
			doc.LoadXml(xml);
			return doc.FirstChild;
		}
	}
}

