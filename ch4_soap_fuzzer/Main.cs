using System;
using System.Linq;
using System.Xml;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace fuzzer
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

			foreach (SoapService service in _wsdl.Services) 
				FuzzService (service);
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
				XElement soapBody = new XElement (soapNS + "Body");
				XElement soapOperation = new XElement (op.Name/*, new XAttribute("xmlns", "http://tempuri.org")*/);

				soapBody.Add (soapOperation);
				int i = 0;
				int j = 0;
				Dictionary<int, Guid> paramMap = new Dictionary<int, Guid> ();
				Dictionary<int, SoapType> typeMap = new Dictionary<int, SoapType> ();
				foreach (SoapMessagePart part in input.Parts) {
					SoapType type = _wsdl.Types.Single (t => t.Name == part.Element.Split (':') [1]);
					foreach (SoapTypeParameter param in type.Parameters) {
						XElement soapParam = new XElement (param.Name);

						if (param.Type.EndsWith ("string")) {
							Guid guid = Guid.NewGuid ();
							paramMap.Add (i, guid);
							soapParam.SetValue (guid.ToString());
							i++;
						}
						soapOperation.Add (soapParam);
					}
					typeMap.Add (j, type);
					j++;
				}

				XDocument soapDoc = new XDocument(new XDeclaration("1.0", "utf-16", "true"),
					new XElement(soapNS + "Envelope",
						new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
						new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
						new XAttribute(XNamespace.Xmlns + "soap", soapNS),
						soapBody));
									
				for (int k = 0; k < i; k++) {
					string testSoap = soapDoc.ToString().Replace (paramMap[k].ToString(), "fd'sa");
					byte[] data = System.Text.Encoding.ASCII.GetBytes (testSoap);

					HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
					req.Headers ["SOAPAction"] = op.SoapAction;
					req.Method = "POST";
					req.ContentType = "text/xml";
					req.ContentLength = data.Length;
					req.Proxy = new WebProxy ("http://127.0.0.1:8080");
					req.GetRequestStream ().Write (data, 0, data.Length);

					string resp = string.Empty;
					try {
						using (StreamReader rdr = new StreamReader(req.GetResponse().GetResponseStream()))
							resp = rdr.ReadToEnd ();
					} catch (WebException ex) {
						using (StreamReader rdr = new StreamReader(ex.Response.GetResponseStream()))
							resp = rdr.ReadToEnd ();

						if (resp.Contains ("syntax error"))
							Console.WriteLine ("Possible SQL injection vector in parameter: " + typeMap[k].Parameters [k].Name);
					}
				}
			}
		}

		static void FuzzHttpGetPort (SoapBinding binding)
		{
			SoapPortType portType = _wsdl.PortTypes.Single (pt => pt.Name == binding.Type.Split (':') [1]);
			List<string> vulnUrls = new List<string> ();
			foreach (SoapBindingOperation op in binding.Operations) {
				Console.WriteLine ("Fuzzing operation: " + op.Name);

				string url = _endpoint + op.Location;
				SoapOperation po = portType.Operations.Single (p => p.Name == op.Name);
				SoapMessage input = _wsdl.Messages.Single (m => m.Name == po.Input.Split (':') [1]);

				Dictionary<string, string> parameters = new Dictionary<string, string> ();
				foreach (SoapMessagePart part in input.Parts) {
					parameters.Add (part.Name, part.Type);
				}

				bool first = true;
				int i = 0;
				foreach (var param in parameters) {
					if (param.Value.EndsWith ("string"))
						url += (first ? "?" : "&") + param.Key + "=fds" + i++;
					if (first)
						first = false;
				}

				Console.WriteLine ("Fuzzing full url: " + url);

				for (int k = 0; k <= i; k++) {
					string testUrl = url.Replace ("fds" + k, "fd'sa");
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create (testUrl);
					string resp = string.Empty;
					try {
						using (StreamReader rdr = new StreamReader(req.GetResponse().GetResponseStream()))
							resp = rdr.ReadToEnd ();
					} catch (WebException ex) {
						using (StreamReader rdr = new StreamReader(ex.Response.GetResponseStream()))
							resp = rdr.ReadToEnd ();

						if (resp.Contains ("syntax error")) {
							if (!vulnUrls.Contains (url))
								vulnUrls.Add (url);

							Console.WriteLine ("Possible SQL injection vector in parameter: " + input.Parts [k].Name);
						}
					}
				}
			}
		}

		static void FuzzHttpPostPort (SoapBinding binding) {
			SoapPortType portType = _wsdl.PortTypes.Single (pt => pt.Name == binding.Type.Split (':') [1]);
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
				int i = 0;
				foreach (var param in parameters) {
					if (param.Value.EndsWith ("string"))
						postParams += (first ? "" : "&") + param.Key + "=fds" + i++;
					if (first)
						first = false;
				}

				for (int k = 0; k <= i; k++) {
					string testParams = postParams.Replace ("fds" + k, "fd'sa");
					byte[] data = System.Text.Encoding.ASCII.GetBytes (testParams);

					HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
					req.Method = "POST";
					req.ContentType = "application/x-www-form-urlencoded";
					req.ContentLength = data.Length;
					req.GetRequestStream ().Write (data, 0, data.Length);

					string resp = string.Empty;
					try {
						using (StreamReader rdr = new StreamReader(req.GetResponse().GetResponseStream()))
							resp = rdr.ReadToEnd ();
					} catch (WebException ex) {
						using (StreamReader rdr = new StreamReader(ex.Response.GetResponseStream()))
							resp = rdr.ReadToEnd ();

						if (resp.Contains ("syntax error"))
							Console.WriteLine ("Possible SQL injection vector in parameter: " + input.Parts [k].Name);
					}
				}
			}
		}
	}
}
	 