using System;
using System.Linq;
using System.Xml;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using sqlmapsharp;
using System.Xml.Linq;

namespace ch9_soap_fuzzer
{
	class MainClass
	{
		private static WSDL _wsdl = null;
		private static string _endpoint = null;

		public static void Main (string[] args) {
			_endpoint = args [0];

			Console.WriteLine ("Fetching the WSDL for service: " + _endpoint);

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (_endpoint + "?WSDL");
			XmlDocument wsdlDoc = new XmlDocument ();

			using (StreamReader rdr = new StreamReader(req.GetResponse().GetResponseStream()))
				wsdlDoc.LoadXml (rdr.ReadToEnd ());

			_wsdl = new WSDL (wsdlDoc);

			Console.WriteLine ("Fetched and loaded the web service description.");

			foreach (SoapService service in _wsdl.Services) {
				FuzzService (service);
			}
		}

		static void FuzzService (SoapService service)
		{
			Console.WriteLine ("Fuzzing service: " + service.Name);

			foreach (SoapPort port in service.Ports) { 
				Console.WriteLine ("Fuzzing " + port.ElementType.Split (':') [0] + " port: " + port.Name);
				SoapBinding binding = _wsdl.Bindings.Single (b => b.Name == port.Binding.Split (':') [1]);

				if (binding.IsHTTP)
					FuzzHttpPort (binding);
				else
					FuzzSoapPort (binding);
			}
		}

		static void FuzzHttpPort (SoapBinding binding) {
			if (binding.Verb == "GET") 
				FuzzHttpGetPort (binding);
			else if (binding.Verb == "POST")
				FuzzHttpPostPort (binding);
			else
				throw new Exception ("Don't know verb: " + binding.Verb);
		}

		static void FuzzSoapPort (SoapBinding binding) {
			SoapPortType portType = _wsdl.PortTypes.Single (pt => pt.Name == binding.Type.Split (':') [1]);

			foreach (SoapBindingOperation op in binding.Operations) {
				Console.WriteLine ("Fuzzing operation: " + op.Name);

				string url = _endpoint;
				SoapOperation po = portType.Operations.Single (p => p.Name == op.Name);
				SoapMessage input = _wsdl.Messages.Single (m => m.Name == po.Input.Split (':') [1]);
				XNamespace soapNS = "http://schemas.xmlsoap.org/soap/envelope/";
				XNamespace xmlNS = op.SoapAction.Replace (op.Name, string.Empty);
				XElement soapBody = new XElement (soapNS + "Body");
				XElement soapOperation = new XElement (xmlNS + op.Name);

				soapBody.Add (soapOperation);

				List<Guid> paramList = new List<Guid> ();
				SoapType type = _wsdl.Types.Single (t => t.Name == input.Parts [0].Element.Split (':') [1]);
				foreach (SoapTypeParameter param in type.Parameters) {
					XElement soapParam = new XElement (xmlNS + param.Name);

					if (param.Type.EndsWith ("string")) {
						Guid guid = Guid.NewGuid ();
						paramList.Add (guid);
						soapParam.SetValue (guid.ToString ());
					}
					soapOperation.Add (soapParam);
				}


				XDocument soapDoc = new XDocument (new XDeclaration ("1.0", "utf-16", "true"),
					new XElement (soapNS + "Envelope",
						new XAttribute (XNamespace.Xmlns + "soap", soapNS),
						new XAttribute ("xmlns", xmlNS),
						soapBody));
				
				int k = 0;					
				Dictionary<Guid, string> vulnValues = new Dictionary<Guid, string>();
				foreach (Guid parm in paramList) {
					string testSoap = soapDoc.ToString ().Replace (parm.ToString(), "fd'sa");
					byte[] data = System.Text.Encoding.ASCII.GetBytes (testSoap);

					HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
					req.Headers ["SOAPAction"] = op.SoapAction;
					req.Method = "POST";
					req.ContentType = "text/xml";
					req.ContentLength = data.Length;
					req.GetRequestStream ().Write (data, 0, data.Length);

					string resp = string.Empty;
					try {
						using (StreamReader rdr = new StreamReader (req.GetResponse ().GetResponseStream ()))
							resp = rdr.ReadToEnd ();
					} catch (WebException ex) {
						using (StreamReader rdr = new StreamReader (ex.Response.GetResponseStream ()))
							resp = rdr.ReadToEnd ();

						if (resp.Contains ("syntax error")) {
							vulnValues.Add(parm, op.SoapAction);
							Console.WriteLine ("Possible SQL injection vector in parameter: " + type.Parameters [k].Name);
						}
					}
					k++;
				}

				foreach (var pair in vulnValues)
					TestPostRequestWithSqlmap(_endpoint, soapDoc.ToString(), pair.Value, pair.Key.ToString());
			}
		}

		static void FuzzHttpGetPort (SoapBinding binding)
		{
			SoapPortType portType = _wsdl.PortTypes.Single (pt => pt.Name == binding.Type.Split (':') [1]);
			List<string[]> vulns = new List<string[]> ();
			foreach (SoapBindingOperation op in binding.Operations) {
				Console.WriteLine ("Fuzzing operation: " + op.Name);

				string url = _endpoint + op.Location;
				SoapOperation po = portType.Operations.Single (p => p.Name == op.Name);
				SoapMessage input = _wsdl.Messages.Single (m => m.Name == po.Input.Split (':') [1]);

				Dictionary<string, string> parameters = new Dictionary<string, string> ();
				foreach (SoapMessagePart part in input.Parts) 
					parameters.Add (part.Name, part.Type);

				bool first = true;
				List<Guid> guidList = new List<Guid> ();
				foreach (var param in parameters) {
					if (param.Value.EndsWith ("string")) {
						Guid guid = Guid.NewGuid ();
						guidList.Add (guid);
						url += (first ? "?" : "&") + param.Key + "=" + guid.ToString();
					}
					if (first)
						first = false;
				}

				Console.WriteLine ("Fuzzing full url: " + url);

				int k = 0;
				foreach(Guid guid in guidList) {
					string testUrl = url.Replace (guid.ToString(), "fd'sa");
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create (testUrl);
					string resp = string.Empty;
					try {
						using (StreamReader rdr = new StreamReader (req.GetResponse ().GetResponseStream ()))
							resp = rdr.ReadToEnd ();
					} catch (WebException ex) {
						using (StreamReader rdr = new StreamReader (ex.Response.GetResponseStream ()))
							resp = rdr.ReadToEnd ();

						if (resp.Contains ("syntax error")) {
							vulns.Add (new string[] { url, input.Parts [k].Name });
							Console.WriteLine ("Possible SQL injection vector in parameter: " + input.Parts [k].Name);
						}
					}
					k++;
				}
			}

			foreach (var vuln in vulns)
				TestGetRequestWithSqlmap (vuln[0], vuln[1]);
		}

		static void FuzzHttpPostPort (SoapBinding binding) {
			SoapPortType portType = _wsdl.PortTypes.Single (pt => pt.Name == binding.Type.Split (':') [1]);
			List<string[]> vulns = new List<string[]> ();
			foreach (SoapBindingOperation op in binding.Operations) {
				Console.WriteLine ("Fuzzing operation: " + op.Name);

				string url = _endpoint + op.Location;
				SoapOperation po = portType.Operations.Single (p => p.Name == op.Name);
				SoapMessage input = _wsdl.Messages.Single (m => m.Name == po.Input.Split (':') [1]);
				Dictionary<string, string> parameters = new Dictionary<string, string> ();

				foreach (SoapMessagePart part in input.Parts) {
					parameters.Add (part.Name, part.Type);
				}

				string postParams = string.Empty;
				bool first = true;
				List<Guid> guids = new List<Guid> ();
				foreach (var param in parameters) {
					if (param.Value.EndsWith ("string")) {
						Guid guid = Guid.NewGuid ();
						postParams += (first ? "" : "&") + param.Key + "=" + guid.ToString ();
						guids.Add (guid);
					}
					if (first)
						first = false;
				}

				int k = 0;
				foreach (Guid guid in guids) {
					string testParams = postParams.Replace (guid.ToString (), "fd'sa");
					byte[] data = System.Text.Encoding.ASCII.GetBytes (testParams);

					HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
					req.Method = "POST";
					req.ContentType = "application/x-www-form-urlencoded";
					req.ContentLength = data.Length;
					req.GetRequestStream ().Write (data, 0, data.Length);

					string resp = string.Empty;
					try {
						using (StreamReader rdr = new StreamReader (req.GetResponse ().GetResponseStream ()))
							resp = rdr.ReadToEnd ();
					} catch (WebException ex) {
						using (StreamReader rdr = new StreamReader (ex.Response.GetResponseStream ()))
							resp = rdr.ReadToEnd ();

						if (resp.Contains ("syntax error")) {
							vulns.Add (new string[] { url, testParams, guid.ToString() });
							Console.WriteLine ("Possible SQL injection vector in parameter: " + input.Parts [k].Name);
						}
					}
					k++;
				}
			}
			foreach (string[] vuln in vulns)
				TestPostRequestWithSqlmap (vuln [0], vuln [1], null, vuln [2]);
		}

		static void TestGetRequestWithSqlmap (string url, string parameter)
		{
			Console.WriteLine("Testing url with sqlmap: " + url);
			using (SqlmapSession session = new SqlmapSession("127.0.0.1", 8775)) {
				using (SqlmapManager manager = new SqlmapManager(session)) {
					string taskid = manager.NewTask();
					var options = manager.GetOptions(taskid);
					options ["url"] = url;
					options ["level"] = 1;
					options ["risk"] = 1;
					options ["dbms"] = "postgresql";
					options ["testParameter"] = parameter;
					options ["proxy"] = "http://192.168.2.207:8080";
					options ["flushSession"] = "true";
					manager.StartTask(taskid, options);

					SqlmapStatus status = manager.GetScanStatus(taskid);
					while (status.Status != "terminated")
					{
						System.Threading.Thread.Sleep(new TimeSpan(0,0,10));
						status = manager.GetScanStatus(taskid);
					}

					List<SqlmapLogItem> logItems = manager.GetLog(taskid);

					foreach (SqlmapLogItem item in logItems)
						Console.WriteLine(item.Message);

					manager.DeleteTask(taskid);
				}
			}
		}

		static void TestPostRequestWithSqlmap(string url, string data, string soapAction, string vulnValue) {
			Console.WriteLine("Testing url with sqlmap: " + url);
			using (SqlmapSession session = new SqlmapSession("127.0.0.1", 8775)) {
				using (SqlmapManager manager = new SqlmapManager(session)) {

					string taskid = manager.NewTask();
					var options = manager.GetOptions(taskid);
					options["url"] = url;
					options ["level"] = 1;
					options ["risk"] = 1;
					options ["dbms"] = "postgresql";
					//options ["testParameter"] = parameter;
					options["proxy"] = "http://192.168.2.207:8080";
					options["data"] = data.Replace(vulnValue, "1*").Trim();
					options["skipUrlEncode"] = "true";
					options["flushSession"] = "true";

					string headers = string.Empty;
					if (!string.IsNullOrEmpty (soapAction))
						headers = "Content-Type: text/xml\nSOAPAction: " + soapAction;
					else 
						headers = "Content-Type: application/x-www-form-urlencoded";
	
					options ["headers"] = headers;

					manager.StartTask(taskid, options);

					SqlmapStatus status = manager.GetScanStatus(taskid);
					while (status.Status != "terminated")
					{
						System.Threading.Thread.Sleep(new TimeSpan(0,0,10));
						status = manager.GetScanStatus(taskid);
					}

					List<SqlmapLogItem> logItems = manager.GetLog(taskid);

					foreach (SqlmapLogItem item in logItems)
						Console.WriteLine(item.Message);

					manager.DeleteTask(taskid);
				}
			}
		}
	}
}
