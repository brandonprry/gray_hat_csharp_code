
using System; 
using System.Net; 
using System.IO; 

namespace SQLiXSS_fuzzer { 
	class MainClass { 
		public static void Main (string[] args) { 
			string url = args [0]; 
			string[] parms = url.Remove (0, url.IndexOf ("?")+1).Split ('&'); 


			foreach (string parm in parms) {
				string xssUrl = url.Replace(parm, parm + "fd<xss>sa"); 
				string sqlUrl = url.Replace(parm, parm + "fd'sa");

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (sqlUrl); 
				request.Method = "GET"; 

				string sqlresp = string.Empty; 
				using (StreamReader rdr = new
					StreamReader(request.GetResponse().GetResponseStream())) {
					sqlresp = rdr.ReadToEnd();
				}

				request = (HttpWebRequest)WebRequest.Create (xssUrl);
				request.Method = "GET";

				string xssresp = string.Empty;
				using (StreamReader rdr = new
					StreamReader(request.GetResponse().GetResponseStream())) {
					xssresp = rdr.ReadToEnd();
				}

				if (xssresp.Contains("<xss>")) {
					Console.WriteLine("Possible XSS point found in parameter: " + parm);
				}

				if (sqlresp.Contains("error in your SQL syntax")) {
					Console.WriteLine ("SQL injection point found in parameter: " + parm);
				}
			}
		}
	}
}