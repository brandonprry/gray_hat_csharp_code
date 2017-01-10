using System;
using MsgPack;
using System.Threading;
using System.Collections.Generic;

namespace ch14_automating_arachni_rpc
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (ArachniRPCSession session = new ArachniRPCSession ("127.0.0.1", 7331, true)) {
				using (ArachniRPCManager manager = new ArachniRPCManager (session)) {
					Console.WriteLine ("Using instance: " + session.InstanceName);
					manager.StartScan ("http://demo.testfire.net/default.aspx");

					bool isRunning = manager.IsBusy ().AsBoolean ();
					List<uint> issues = new List<uint> ();
					DateTime start = DateTime.Now;
					Console.WriteLine ("Starting scan at " + start.ToLongTimeString ());
					while (isRunning) {
						var progress = manager.GetProgress (issues);
						foreach (MessagePackObject p in progress.AsDictionary()["issues"].AsEnumerable()) {
							MessagePackObjectDictionary dict = p.AsDictionary ();
							Console.WriteLine ("Issue found: " + dict ["name"].AsString ());
							issues.Add (dict ["digest"].AsUInt32());
						}
						Thread.Sleep (10000);
						isRunning = manager.IsBusy ().AsBoolean ();
					}
					DateTime end = DateTime.Now;
					Console.WriteLine ("Finishing scan at " + end.ToLongTimeString () + ". Scan took " + ((end - start).ToString ()) + ".");
				}
			}
		}
	}
}
