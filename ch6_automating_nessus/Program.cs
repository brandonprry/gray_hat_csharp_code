using System;

namespace ch6_automating_nessus
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (NessusSession session = new NessusSession ("192.168.1.28")) {
				bool loggedIn;

				session.Authenticate ("admin", "password", 1234, out loggedIn);

				if (loggedIn)
					Console.WriteLine ("Authenticated");
				else
					Console.WriteLine ("Not authenticated");
			}
		}
	}
}
