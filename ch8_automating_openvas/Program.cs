using System;
using openvassharp;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Linq;

namespace ch8_automating_openvas
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (OpenVASSession session = new OpenVASSession ("admin", "password", "192.168.1.5")) {
				using (OpenVASManager manager = new OpenVASManager (session)) {

					XDocument target = manager.CreateSimpleTarget ("192.168.1.0/24", Guid.NewGuid ().ToString ());
					string targetID = target.Root.Attribute ("id").Value;

					XDocument configs = manager.GetScanConfigurations ();

					string discoveryConfigID = string.Empty;
					foreach (XElement node in configs.Descendants(XName.Get("name"))) {
						if (node.Value == "Discovery") {
							discoveryConfigID = node.Parent.Attribute ("id").Value;
							break;
						}
					}

					XDocument task = manager.CreateSimpleTask (Guid.NewGuid ().ToString (), string.Empty, new Guid (discoveryConfigID), new Guid (targetID));

					Guid taskID = new Guid (task.Root.Attribute ("id").Value);

					manager.StartTask (taskID);

					XDocument status = manager.GetTasks (taskID);

					while (status.Descendants ("status").First ().Value != "Done") {
						Thread.Sleep (500);
						Console.WriteLine (status.Descendants (XName.Get ("progress")).First ().Nodes ().OfType<XText> ().First ().Value);
						status = manager.GetTasks (taskID);
					}

					XDocument results = manager.GetTaskResults (taskID);

					Console.WriteLine (results.ToString ());
				}
			}
		}
	}

}
