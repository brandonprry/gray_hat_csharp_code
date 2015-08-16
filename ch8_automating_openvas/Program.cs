using System;
using System.Xml.Linq;
using System.Threading;
using System.Linq;
using System.Xml.XPath;
using System.Xml;


namespace ch8_automating_openvas
{
	class MainClass
	{
		public static void Main (string[] args)
		{

			using (OpenVASSession session = new OpenVASSession ("admin", "admin", "192.168.1.19")) {
				using (OpenVASManager manager = new OpenVASManager (session)) {

					XDocument target = manager.CreateSimpleTarget ("192.168.1.31", Guid.NewGuid ().ToString ());
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
						Console.Clear ();
						string percentComplete = status.Descendants (XName.Get ("progress")).First ().Nodes ().OfType<XText> ().First ().Value;
						Console.WriteLine ("The scan is " + percentComplete + "% done.");
						status = manager.GetTasks (taskID);
					}

					XDocument results = manager.GetTaskResults (taskID);

					var name = results.XPathSelectElements ("//get_reports_response/report/report/results");

					Console.WriteLine (name.ToString ());

					foreach (var elem in name) {
						Console.WriteLine (elem.ToString ());
					}

				}
			}
		}
	}
}
