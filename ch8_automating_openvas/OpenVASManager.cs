using System;
using System.Xml;
using System.Xml.Linq;

namespace ch7_automating_openvas
{
	public class OpenVASManager : IDisposable
	{
		private OpenVASSession _session;

		public OpenVASManager (OpenVASSession session)
		{
			if (session != null)
				_session = session;
		}

		public XDocument GetVersion ()
		{
			return _session.ExecuteCommand (XDocument.Parse ("<get_version />"));
		}

		public XDocument GetScanConfigurations ()
		{
			return _session.ExecuteCommand (XDocument.Parse ("<get_configs />"), true);
		}

		public XDocument CreateSimpleTarget (string cidrRange, string targetName)
		{
			XDocument createTargetXML = new XDocument (
				                            new XElement ("create_target",
					                            new XElement ("name", targetName),
					                            new XElement ("hosts", cidrRange)));
			
			return _session.ExecuteCommand (createTargetXML, true);
		}

		public XDocument CreateSimpleTask (string name, string comment, Guid configID, Guid targetID)
		{

			XDocument createTaskXML = new XDocument (
				                          new XElement ("create_task",
					                          new XElement ("name", name),
					                          new XElement ("comment", comment),
					                          new XElement ("config", 
						                          new XAttribute ("id", configID.ToString ())),
					                          new XElement ("target",
						                          new XAttribute ("id", targetID.ToString ()))));


			return _session.ExecuteCommand (createTaskXML, true);
		}

		public XDocument StartTask (Guid taskID)
		{
			XDocument startTaskXML = new XDocument (
				                         new XElement ("start_task",
					                         new XAttribute ("task_id", taskID.ToString ())));
			
			return _session.ExecuteCommand (startTaskXML, true);
		}

		public XDocument GetTasks (Guid? taskID = null)
		{

			if (taskID != null)
				return _session.ExecuteCommand (new XDocument (
					new XElement ("get_tasks", 
						new XAttribute ("task_id", taskID.ToString ()))), true);

			return _session.ExecuteCommand (XDocument.Parse ("<get_tasks />"), true);
		}

		public XDocument GetTaskResults (Guid taskID)
		{
			XDocument getTaskResultsXML = new XDocument (
				                              new XElement ("get_results",
					                              new XAttribute ("task_id", taskID.ToString ())));
			
			return _session.ExecuteCommand (getTaskResultsXML, true);
		}

		public void Dispose ()
		{
			_session.Dispose ();
		}
		
	}
}
