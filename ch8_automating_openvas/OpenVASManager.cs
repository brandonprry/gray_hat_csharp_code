using System;
using System.Xml;

namespace openvassharp
{
	public class OpenVASManager : IDisposable
	{
		private OpenVASManagerSession _session;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoAssess.Data.OpenVAS.OpenVASManager"/> class.
		/// </summary>
		public OpenVASManager ()
		{
			_session = null;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoAssess.Data.OpenVAS.OpenVASManager"/> class.
		/// </summary>
		/// <param name='session'>
		/// Session.
		/// </param>/
		public OpenVASManager(OpenVASManagerSession session)
		{
			if (session != null)
				_session = session;
		}
		
		/// <summary>
		/// Gets a value indicating whether this instance is authenticated.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
		/// </value>
		public bool IsAuthenticated { get { return _session.IsAuthenticated; }}
		
		/// <summary>
		/// Creates an OpenVAS agent.
		/// </summary>
		/// <returns>
		/// XML Document response from OpenVAS.
		/// </returns>
		/// <param name='installer'>
		/// Installer.
		/// </param>
		/// <param name='signature'>
		/// Signature.
		/// </param>
		/// <param name='name'>
		/// Name.
		/// </param>
		public XmlDocument CreateAgent(string installer, string signature, string name)
		{
			CheckSession();
			
			string command = "<create_agent>";
			
			command = command + "<installer>" + installer;
			
			if (!string.IsNullOrEmpty(signature))
				command = command + "<signature>" + signature+ "</signature>";
			
			command = command + "</installer>";
			command = command + "<name>" + name + "</name>";
			command = command + "</create_agent>";
			
			XmlDocument cmd = new XmlDocument();
			cmd.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(cmd);
			
			return response;
		}
		
		/// <summary>
		/// Create a config from an RC file
		/// </summary>
		/// <returns>
		/// XML response from OpenVAS host.
		/// </returns>
		/// <param name='name'>
		/// Name of the config
		/// </param>
		/// <param name='rcfile'>
		/// Rcfile with config information
		/// </param>
		public XmlDocument CreateConfigByRC(string name, string rcfile)
		{
			CheckSession();
			
			string command = "<create_config>";
			command = command + "<rcfile>" + rcfile + "</rcfile>";
			command = command + "<name>" + name + "</name>";
			command = command + "</create_config>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response =  _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Creates the config by copying another config
		/// </summary>
		/// <returns>
		/// XML response from OpenVAS host.
		/// </returns>
		/// <param name='name'>
		/// Name of the config to be created
		/// </param>
		/// <param name='id'>
		/// ID of config to be copied.
		/// </param>
		public XmlDocument CreateConfigByCopy(string name, string id)
		{
			CheckSession();			
			
			string command = "<create_config>";
			command = command + "<copy>" + id + "</copy>";
			command = command + "<name>" + name + "</name>";
			command = command + "</create_config>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response =  _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Creates the config by a previous response.
		/// </summary>
		/// <returns>
		/// XML response from the OpenVAS host.
		/// </returns>
		/// <param name='configResponse'>
		/// Config response.
		/// </param>
		public XmlDocument CreateConfigByResponse(XmlDocument configResponse)
		{
			CheckSession();
			
			string command = "<create_config>";
			command = command + configResponse.OuterXml;
			command = command + "</create_config>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response =  _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Creates an OpenVAS escalator.
		/// </summary>
		/// <returns>
		/// XML response from OpenVAS host.
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='condition'>
		/// Condition.
		/// </param>
		/// <param name='conditionData'>
		/// Condition data.
		/// </param>
		/// <param name='conditionName'>
		/// Condition name.
		/// </param>
		public XmlDocument CreateEscalator(string name, string condition, string conditionData, string conditionName)
		{
			CheckSession();
			
			string command = "<create_escalator>";
			command = command + "<name>" + name + "</name>";
			command = command + "<condition>" + condition;
			command = command + "<data>" + conditionData;
			
			if (!string.IsNullOrEmpty(conditionName))
				command = command + "<name>" + conditionName + "</name>";
			
			command = command + "</data></condition></create_escalator>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Creates an OpenVAS local security check credential.
		/// </summary>
		/// <returns>
		/// XML response from OpenVAS server.
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='username'>
		/// Username.
		/// </param>
		/// <param name='password'>
		/// Password.
		/// </param>
		/// <param name='comment'>
		/// Comment.
		/// </param>
		public XmlDocument CreateLSCCredential(string name, string username, string password, string comment)
		{
			CheckSession();
			
			string command = "<create_lsc_credential>";
			
			command = command + "<name>" + name + "</name>";
			command = command + "<login>" + username + "</login>";
			command = command + "<password>" + password + "</password>";
			command = command + "<comment>" + comment + "</comment>";
			command = command + "</create_lsc_credential>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Creates a note related to a report or task in OpenVAS
		/// </summary>
		/// <returns>
		/// The note.
		/// </returns>
		/// <param name='content'>
		/// Content.
		/// </param>
		/// <param name='oid'>
		/// Oid.
		/// </param>
		/// <param name='comment'>
		/// Comment.
		/// </param>
		/// <param name='hosts'>
		/// Hosts.
		/// </param>
		/// <param name='port'>
		/// Port.
		/// </param>
		/// <param name='reportID'>
		/// Report ID.
		/// </param>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
		/// <param name='threatLevel'>
		/// Threat level.
		/// </param>
		public XmlDocument CreateNote(string content, string oid, string comment, string hosts, string port, string reportID, string taskID, string threatLevel)
		{
			CheckSession();
			
			string command = "<create_note>";
			
			command = command + "<text>" + content + "</text>";
			
			if (!string.IsNullOrEmpty(oid))
				command = command + "<nvt>" + oid + "</nvt>";
			
			command = command + "<comment>" + comment + "</comment>";
			command = command + "<hosts>" + hosts + "</hosts>";
			command = command + "<port>" + port + "</port>";
			
			if (!string.IsNullOrEmpty(reportID))
				command = command + "<report>" + reportID + "</report>";
			
			if (!string.IsNullOrEmpty(taskID))
				command = command + "<task>" + taskID + "</task>";
			
			command = command + "<threat>" + threatLevel + "</threat></create_note>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Creates the override.
		/// </summary>
		/// <returns>
		/// The override.
		/// </returns>
		/// <param name='content'>
		/// Content.
		/// </param>
		/// <param name='oid'>
		/// Oid.
		/// </param>
		/// <param name='comment'>
		/// Comment.
		/// </param>
		/// <param name='hosts'>
		/// Hosts.
		/// </param>
		/// <param name='newThreat'>
		/// New threat.
		/// </param>
		/// <param name='port'>
		/// Port.
		/// </param>
		/// <param name='reportID'>
		/// Report ID.
		/// </param>
		/// <exception cref='NotImplementedException'>
		/// Is thrown when a requested operation is not implemented for a given type.
		/// </exception>
		public XmlDocument CreateOverride(string content, string oid, string comment, string hosts, string newThreatLevel, string port, string reportID)
		{
			CheckSession();
			
			string command = "<create_override>";
			
			command = command + "<text>" + content + "</text>";
			
			if (!string.IsNullOrEmpty(oid))
				command = command + "<nvt>" + oid + "</nvt>";
			
			command = command + "<comment>" + command + "</comment>";
			command = command + "<hosts>" + hosts + "</hosts>";
			command = command + "<new_threat>" + newThreatLevel + "</new_threat>";
			command = command + "<port>" + port + "</port>";
			
			if (!string.IsNullOrEmpty(reportID))
				command = command + "<report>" + reportID + "</report>";
			
			//command = command + "<
			
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Creates the report format.
		/// </summary>
		/// <returns>
		/// The report format.
		/// </returns>
		/// <exception cref='NotImplementedException'>
		/// Is thrown when a requested operation is not implemented for a given type.
		/// </exception>
		public XmlDocument CreateReportFormat(XmlDocument getReportFormatReposonse)
		{
			CheckSession();
			
			//string command = "<create_report_format>";
			
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Creates the schedule.
		/// </summary>
		/// <returns>
		/// The schedule.
		/// </returns>
		/// <exception cref='NotImplementedException'>
		/// Is thrown when a requested operation is not implemented for a given type.
		/// </exception>
		public XmlDocument CreateSchedule(string name, DateTime firstRun, int duration, string durationUnit, int period, string periodUnit)
		{
			CheckSession();
			
			string command = "<create_schedule>";
			
			command = command + "<name>" + name + "</name>";
			
			command = command + "<first_run>";
			command = command + "<day_of_month>" + firstRun.Day + "</day_of_run>";
			command = command + "<hour>" + firstRun.Hour + "</hour>";
			command = command + "<minute>" + firstRun.Minute + "</minute>";
			command = command + "<month>" + firstRun.Month + "</month>";
			command = command + "<year>" + firstRun.Year + "</year>";
			command = command + "</first_run>";
			
			command = command + "<duration>" + duration;
			command = command + "<unit>" + durationUnit + "</unit>";
			command = command + "</duration>";
			
			command = command + "<period>" + period;
			command = command + "<unit>" + periodUnit + "</unit>";
			command = command + "</period>";
			
			command = command + "</create_schedule>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
			
		}
		
		/// <summary>
		/// Creates the slave.
		/// </summary>
		/// <returns>
		/// The slave.
		/// </returns>
		/// <exception cref='NotImplementedException'>
		/// Is thrown when a requested operation is not implemented for a given type.
		/// </exception>
		public XmlDocument CreateSlave(string name, string comment, string host, int port, string username, string password)
		{
			CheckSession();
			
			string command = "<create_slave>";
			
			command = command + "<name>" + name + "</name>";
			command = command + "<comment>" + comment + "</comment>";
			command = command + "<host>" + host + "</host>";
			command = command + "<login>" + username + "</login>";
			command = command + "<password>" + password + "</password>";
			command = command + "</create_slave>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Creates the target.
		/// </summary>
		/// <returns>
		/// The target.
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='comment'>
		/// Comment.
		/// </param>
		/// <param name='hosts'>
		/// Hosts.
		/// </param>
		/// <param name='smbCredentialsID'>
		/// Smb credentials ID.
		/// </param>
		/// <param name='sshCredentialsID'>
		/// Ssh credentials ID.
		/// </param>
		/// <param name='portRange'>
		/// Port range.
		/// </param>
		public XmlDocument CreateTarget(string name, string comment, string hosts, string smbCredentialsID, string sshCredentialsID, string portRange)
		{
			CheckSession();
			
			string command = "<create_target>";
			
			command = command + "<name>" + name + "</name>";
			command = command + "<comment>" + comment + "</comment>";
			command = command + "<hosts>" + hosts + "</hosts>";
			
			if (!string.IsNullOrEmpty(smbCredentialsID))
				command = command + "<smb_lsc_credential>" + smbCredentialsID+ "</smb_lsc_credential>";
			
			if (!string.IsNullOrEmpty(sshCredentialsID))
				command = command + "<ssh_lsc_credential>" + sshCredentialsID + "</ssh_lsc_credential>";
			
			if (!string.IsNullOrEmpty(portRange))
				command = command + "<port_range>" + portRange + "</port_range>";
			
			command = command + "</create_target>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Creates the task.
		/// </summary>
		/// <returns>
		/// The task.
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='comment'>
		/// Comment.
		/// </param>
		/// <param name='configID'>
		/// Config ID.
		/// </param>
		/// <param name='targetID'>
		/// Target ID.
		/// </param>
		/// <param name='escalatorID'>
		/// Escalator ID.
		/// </param>
		/// <param name='scheduleID'>
		/// Schedule ID.
		/// </param>
		/// <param name='slaveID'>
		/// Slave ID.
		/// </param>
		/// <exception cref='Exception'>
		/// Represents errors that occur during application execution.
		/// </exception>
		public XmlDocument CreateTask(string name, string comment, string configID, string targetID, string escalatorID, string scheduleID, string slaveID)
		{
			
			CheckSession();
			
			string command = "<create_task>";
			
			command = command + "<name>" + name + "</name>";
			command = command + "<comment>" + comment + "</comment>";
			
			if (string.IsNullOrEmpty(configID))
				throw new Exception("Need a config for the task at hand.");
			
			command = command + "<config id=\"" + configID + "\" />";
			
			if (string.IsNullOrEmpty(targetID))
				throw new Exception("Need a target.");
			
			command = command + "<target id=\"" + targetID + "\" />";
			
			if (!string.IsNullOrEmpty(escalatorID))
				command = command + "<escalator id=\"" + escalatorID + "\" />";
				
			if (!string.IsNullOrEmpty(scheduleID))
				command = command + "<schedule id=\"" + scheduleID + "\" />";
				
			if (!string.IsNullOrEmpty(slaveID))
				command = command + "<slave id=\"" + slaveID + "\" />";
			
			command = command + "</create_task>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the agent.
		/// </summary>
		/// <returns>
		/// The agent.
		/// </returns>
		/// <exception cref='NotImplementedException'>
		/// Is thrown when a requested operation is not implemented for a given type.
		/// </exception>
		public XmlDocument DeleteAgent()
		{
			CheckSession();
			
			
			
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Deletes the config.
		/// </summary>
		/// <returns>
		/// The config.
		/// </returns>
		/// <param name='configID'>
		/// Config ID.
		/// </param>
		public XmlDocument DeleteConfig(string configID)
		{
			CheckSession();
			
			string command = "<delete_config config_id=\"" + configID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the escalator.
		/// </summary>
		/// <returns>
		/// The escalator.
		/// </returns>
		/// <param name='escalatorID'>
		/// Escalator ID.
		/// </param>
		public XmlDocument DeleteEscalator(string escalatorID)
		{
			CheckSession();
			
			string command = "<delete_escalator escalator_id=\"" + escalatorID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the LSC credential.
		/// </summary>
		/// <returns>
		/// The LSC credential.
		/// </returns>
		/// <param name='credentialsID'>
		/// Credentials ID.
		/// </param>
		public XmlDocument DeleteLSCCredential(string credentialsID)
		{
			CheckSession();
			
			string command = "<delete_lsc_credential lsc_credential_id=\"" + credentialsID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the note.
		/// </summary>
		/// <returns>
		/// The note.
		/// </returns>
		/// <param name='noteID'>
		/// Note ID.
		/// </param>
		public XmlDocument DeleteNote(string noteID)
		{
			CheckSession();
			
			string command = "<delete_note note_id=\"" + noteID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the override.
		/// </summary>
		/// <returns>
		/// The override.
		/// </returns>
		/// <param name='overrideID'>
		/// Override ID.
		/// </param>
		public XmlDocument DeleteOverride(string overrideID)
		{
			CheckSession();
			
			string command = "<delete_override override_id=\"" + overrideID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the report.
		/// </summary>
		/// <returns>
		/// The report.
		/// </returns>
		/// <param name='reportID'>
		/// Report ID.
		/// </param>
		public XmlDocument DeleteReport(string reportID) 
		{
			CheckSession();
			
			string command = "<delete_report report_id=\"" + reportID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the report format.
		/// </summary>
		/// <returns>
		/// The report format.
		/// </returns>
		/// <param name='formatID'>
		/// Format ID.
		/// </param>
		public XmlDocument DeleteReportFormat(string formatID)
		{
			CheckSession();
			
			string command = "<delete_format format_id=\"" + formatID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the schedule.
		/// </summary>
		/// <returns>
		/// The schedule.
		/// </returns>
		/// <param name='scheduleID'>
		/// Schedule ID.
		/// </param>
		public XmlDocument DeleteSchedule(string scheduleID)
		{
			CheckSession();
			
			string command = "<delete_schedule schedule_id=\"" + scheduleID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the slave.
		/// </summary>
		/// <returns>
		/// The slave.
		/// </returns>
		/// <param name='slaveID'>
		/// Slave ID.
		/// </param>
		public XmlDocument DeleteSlave(string slaveID)
		{
			CheckSession();
			
			string command = "<delete_slave slave_id=\"" + slaveID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the target.
		/// </summary>
		/// <returns>
		/// The target.
		/// </returns>
		/// <param name='targetID'>
		/// Target ID.
		/// </param>
		public XmlDocument DeleteTarget(string targetID)
		{
			CheckSession();
			
			string command = "<delete_target target_id=\"" + targetID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Deletes the task.
		/// </summary>
		/// <returns>
		/// The task.
		/// </returns>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
		public XmlDocument DeleteTask(string taskID)
		{
			CheckSession();
			
			string command = "<delete_task task_id=\"" + taskID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the agents.
		/// </summary>
		/// <returns>
		/// The agents.
		/// </returns>
		/// <param name='agentID'>
		/// Agent ID.
		/// </param>
		/// <param name='format'>
		/// Format.
		/// </param>
		/// <param name='sortOrder'>
		/// Sort order.
		/// </param>
		/// <param name='sortField'>
		/// Sort field.
		/// </param>
		public XmlDocument GetAgents(string agentID, string format, string sortOrder, string sortField)
		{
			CheckSession();
			
			string command = "<get_agents ";
			
			if (!string.IsNullOrEmpty(agentID))
				command = command + "agentid=\"" + agentID+ "\" ";
			
			if (!string.IsNullOrEmpty(format))
				command = command + "format=\"" + format + "\" ";
			
			if (!string.IsNullOrEmpty(sortOrder))
				command = command + "sort_order=\"" + sortOrder + "\" ";
			
			if (!string.IsNullOrEmpty(sortField))
				command = command + "sort_field=\"" + sortField + "\" ";
			
			command = command + "/>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the configs.
		/// </summary>
		/// <returns>
		/// The configs.
		/// </returns>
		/// <param name='configID'>
		/// Config ID.
		/// </param>
		/// <param name='families'>
		/// Families.
		/// </param>
		/// <param name='preferences'>
		/// Preferences.
		/// </param>
		/// <param name='sortOrder'>
		/// Sort order.
		/// </param>
		/// <param name='sortField'>
		/// Sort field.
		/// </param>
		public XmlDocument GetConfigs(string configID, bool families, bool preferences, string sortOrder, string sortField)
		{
			CheckSession();
			
			string command = "<get_configs ";
			
			if (!string.IsNullOrEmpty(configID))
				command = command + "config_id=\"" + configID + "\" ";
			
			command = command + "families=\"" + (families ? "1" : "0") + "\" ";
			
			command = command + "preferences=\"" + (preferences ? "1" : "0") + "\" ";
			
			if (!string.IsNullOrEmpty(sortOrder))
				command = command + "sort_order=\"" + sortOrder + "\" ";
			
			if (!string.IsNullOrEmpty(sortField))
				command = command + "sort_field=\"" + sortField + "\" ";
				
			command = command + "/>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the dependencies.
		/// </summary>
		/// <returns>
		/// The dependencies.
		/// </returns>
		/// <param name='oid'>
		/// Oid.
		/// </param>
		public XmlDocument GetDependencies(string oid)
		{
			CheckSession();
			
			string command = "<get_dependencies ";
			
			if (!string.IsNullOrEmpty(oid))
				command = command + "nvt_oid=\"" + oid + "\" ";
			
			command = command + "/>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the escalators.
		/// </summary>
		/// <returns>
		/// The escalators.
		/// </returns>
		/// <param name='escalatorID'>
		/// Escalator ID.
		/// </param>
		/// <param name='sortOrder'>
		/// Sort order.
		/// </param>
		/// <param name='sortField'>
		/// Sort field.
		/// </param>
		public XmlDocument GetEscalators(string escalatorID, string sortOrder, string sortField)	
		{
			CheckSession();
			
			string command = "<get_escalators ";
			
			if (!string.IsNullOrEmpty(escalatorID))
				command = command + "escalator_id=\"" + escalatorID + "\" ";
			
			if (!string.IsNullOrEmpty(sortOrder))
				command = command + "sort_order=\"" + sortOrder + "\" ";
			
			if (!string.IsNullOrEmpty(sortField))
				command = command + "sort_field=\"" + sortField + "\" ";
			
			command = command + "/>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the local security check credentials.
		/// </summary>
		/// <returns>
		/// The local security check credentials.
		/// </returns>
		/// <param name='lscCredentialsID'>
		/// Lsc credentials.
		/// </param>
		/// <param name='format'>
		/// Format.
		/// </param>
		/// <param name='sortOrder'>
		/// Sort order.
		/// </param>
		/// <param name='sortField'>
		/// Sort field.
		/// </param>
		public XmlDocument GetLocalSecurityCheckCredentials(string lscCredentialsID, string format, string sortOrder, string sortField)
		{
			CheckSession();
			
			string command = "<get_lsc_credentials ";
			
			if (!string.IsNullOrEmpty(lscCredentialsID))
				command = command + "lsc_credential_id=\"" + lscCredentialsID + "\" ";
			
			if (!string.IsNullOrEmpty(format))
				command = command + "format=\"" + format + "\" ";
			
			if (!string.IsNullOrEmpty(sortOrder))
				command = command + "sort_order=\"" + sortOrder + "\" ";
			
			if (!string.IsNullOrEmpty(sortField))
				command = command + "sort_field=\"" + sortField + "\" ";
			
			command = command + "/>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the notes.
		/// </summary>
		/// <returns>
		/// The notes.
		/// </returns>
		/// <param name='noteID'>
		/// Note ID.
		/// </param>
		/// <param name='nvtOID'>
		/// Nvt OID.
		/// </param>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
		/// <param name='details'>
		/// Details.
		/// </param>
		/// <param name='result'>
		/// Result.
		/// </param>
		/// <param name='sortOrder'>
		/// Sort order.
		/// </param>
		/// <param name='sortField'>
		/// Sort field.
		/// </param>
		public XmlDocument GetNotes(string noteID, string nvtOID, string taskID, bool details, bool result, string sortOrder, string sortField)
		{
			CheckSession();
			
			string command = "<get_notes ";
			
			if (!string.IsNullOrEmpty(noteID))
				command = command + "note_id=\"" + noteID + "\" ";
			
			if (!string.IsNullOrEmpty(taskID))
				command = command + "task_id=\"" + taskID + "\" ";
				
			if (!string.IsNullOrEmpty(nvtOID))
				command = command + "nvt_oid=\"" + nvtOID + "\" ";
			
			command = command + "details=\"" + (details ? "1" : "0") + "\" ";
			
			command = command + "result=\"" + (result ? "1" : "0") + "\" ";
			
			if (!string.IsNullOrEmpty(sortOrder))
				command = command + "sort_order=\"" + sortOrder + "\" ";
			
			if (!string.IsNullOrEmpty(sortField))
				command = command + "sort_field=\"" + sortField + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the NV ts.
		/// </summary>
		/// <returns>
		/// The NV ts.
		/// </returns>
		/// <param name='nvtOID'>
		/// Nvt OID.
		/// </param>
		/// <param name='details'>
		/// Details.
		/// </param>
		/// <param name='preferences'>
		/// Preferences.
		/// </param>
		/// <param name='prefCount'>
		/// Preference count.
		/// </param>
		/// <param name='timeout'>
		/// Timeout.
		/// </param>
		/// <param name='configID'>
		/// Config ID.
		/// </param>
		/// <param name='family'>
		/// Family.
		/// </param>
		/// <param name='sortOrder'>
		/// Sort order.
		/// </param>
		/// <param name='sortField'>
		/// Sort field.
		/// </param>
		public XmlDocument GetNVTs(string nvtOID, bool details, bool preferences, bool prefCount, bool timeout, string configID, string family, string sortOrder, string sortField)
		{
			CheckSession();
			
			string command = "<get_nvts ";
			
			if (!string.IsNullOrEmpty(nvtOID))
				command = command + "nvt_oid=\"" + nvtOID + "\" ";
			
			if (!string.IsNullOrEmpty(configID))
				command = command + "config_id=\"" + configID + "\" ";
			
			command = command + "details=\"" + (details ? "1" : "0") + "\" ";
			command = command + "preferences=\"" + (preferences ? "1" : "0") + "\" ";
			command = command + "preference_count=\"" + (prefCount ? "1" : "0") + "\" ";
			command = command + "timeout=\"" + (timeout ? "1" : "0") + "\" ";
			
			if (!string.IsNullOrEmpty(family))
				command = command + "family=\"" + family + "\" ";
			
			if (!string.IsNullOrEmpty(sortOrder))
			command = command + "sort_order=\"" + sortOrder + "\" ";
			
			if (!string.IsNullOrEmpty(sortField))
				command = command + "sort_field=\"" + sortField + "\" ";
			
			command = command + "/>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the NVT families.
		/// </summary>
		/// <returns>
		/// The NVT families.
		/// </returns>
		/// <param name='sortOrder'>
		/// Sort order.
		/// </param>
		public XmlDocument GetNVTFamilies(string sortOrder)
		{
			CheckSession();
			
			string command = "<get_nvt_families ";
			
			if (!string.IsNullOrEmpty(sortOrder))
				command= "sort_order=\""+ sortOrder + "\" ";
			
			command = command + "/>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the NVT feed checksum.
		/// </summary>
		/// <returns>
		/// The NVT feed checksum.
		/// </returns>
		/// <param name='algorithm'>
		/// Algorithm.
		/// </param>
		public XmlDocument GetNVTFeedChecksum(string algorithm)
		{
			CheckSession();
			
			string command = "<get_nvt_feed_checksum algorithm=\"" + algorithm + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		public XmlDocument GetOverrides()
		{
			throw new NotImplementedException();
		}
		
		public XmlDocument GetPreferences()
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Gets the report.
		/// </summary>
		/// <returns>
		/// The report.
		/// </returns>
		/// <param name='reportID'>
		/// Report ID.
		/// </param>
		public XmlDocument GetReportByID(string reportID)
		{
			CheckSession();
			
			string command = "<get_reports report_id=\"" + reportID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// 
		///@report_id (uuid) ID of single report to get.
		///@format_id (uuid) ID of required report format.
		///@first_result (integer) First result to get.
		///@max_results (integer) Maximum number of results to get.
		///@sort_order (sort_order)
		///@sort_field (text)
		///@levels (levels) Which threat levels to include in the report.
		///@search_phrase (text) A string that all results in the report must contain.
		///@min_cvss_base (integer) Minimum CVSS base of results in report.
		///@notes (boolean) Whether to include notes in the report.
		///@note_details (boolean) If notes are included, whether to include note details.
		///@overrides (boolean) Whether to include overrides in the report.
		///@override_details (boolean) If overrides are included, whether to include override details.
		///@result_hosts_only (boolean) Whether to include only those hosts that have results.
		/// </summary>
		/// <returns>
		/// A <see cref="List<OpenVASReport>"/>
		/// </returns>
		public XmlDocument GetReports(string reportID, string formatID, int? firstResult, int? maxResults, string sortOrder, string sortField, 
		         string levels, string searchPhrase, int? minCVSS, bool? notes, bool? notesDetails, bool? overrides, bool? overrideDetails, bool? resultHostsOnly)
		{
			CheckSession();
			
			string command = "<get_reports ";
			
			if (!string.IsNullOrEmpty(reportID))
				command = command + "report_id=\"" + reportID + "\" ";
			
			if (!string.IsNullOrEmpty(formatID))
				command = command + "format_id=\"" + formatID + "\" ";
			
			if (firstResult.HasValue)
				command = command + "first_result=\"" + firstResult + "\" ";
			
			if (maxResults.HasValue)
				command = command + "max_results=\"" + maxResults + "\" ";
			
			if (string.IsNullOrEmpty(sortOrder))
				command = command + "sort_order=\"" + sortOrder + "\" ";
			
			if (string.IsNullOrEmpty(sortField))
				command = command + "sort_field=\"" + sortField + "\" ";
			
			if (!string.IsNullOrEmpty(levels))
				command = command + "levels=\"" + levels + "\" ";
			
			if (!string.IsNullOrEmpty(searchPhrase))
				command = command + "search_phrase=\"" + searchPhrase + "\" ";
			
			if (minCVSS.HasValue)
				command = command + "min_cvss_base=\"" + minCVSS + "\" ";
			
			if (notes.HasValue)
				command = command + "notes=\"" + notes + "\" ";
			
			if (notesDetails.HasValue)
				command = command + "notes_details=\"" + notesDetails + "\" ";
			
			if (overrides.HasValue)
				command = command + "overrides=\"" + overrides + "\" ";
			
			if (resultHostsOnly.HasValue)
				command = command + "result_hosts_only=\"" + resultHostsOnly + "\" ";
			
			command = command + " />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		/// <summary>
		/// 
		///@report_format_id (uuid) ID of single report format to get.
		///@export (boolean) Whether to get report format in importable form.
		///@params (boolean) Whether to include report format parameters.
		///@sort_order (sort_order)
		///@sort_field (text)
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="List<OpenVASReportFormat>"/>
		/// </returns>
		public XmlDocument GetReportFormats(string reportFormatID, bool? export, bool? parameters, string sortOrder, string sortField)
		{
			CheckSession();
			
			string command = "<get_report_formats ";
			
			if (!string.IsNullOrEmpty(reportFormatID))
				command = command + "report_format_id=\"" + reportFormatID + "\" ";
			
			if(export.HasValue)
				command = command + "export=\"" + export + "\" ";
			
			if (parameters.HasValue)
				command = command + "params=\"" + parameters + "\" ";
			
			if (!string.IsNullOrEmpty(sortOrder))
				command = command + "sort_order=\"" + sortOrder + "\" ";
			
			if (!string.IsNullOrEmpty(sortField))
				command = command + "sort_field=\"" + sortField + "\" ";
			
			command = command + "/>";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		public void GetResults()
		{
			CheckSession();
			
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// 
		///@schedule_id (uuid) ID of single schedule to get.
		///@details (boolean) Whether to include full schedule details.
		///@sort_order (sort_order)
		///@sort_field (text)
		/// </summary>
		/// <returns>
		/// A <see cref="List<OpenVASSchedule>"/>
		/// </returns>
		public XmlDocument GetSchedules(string scheduleID, bool details, string sortOrder, string sortField)
		{
			CheckSession();
			
			string command = "<get_schedules schedule_id=\"" + scheduleID + "\" ";
			command = command + "details=\"" + details + "\" sort_order=\"" + sortOrder + "\" sort_field=\"" + sortField + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		public XmlDocument GetSlaves()
		{
			throw new NotImplementedException();
		}
		
		public XmlDocument GetSystemReports()
		{
			throw new NotImplementedException();
		}
		
		public XmlDocument GetTargetLocators()
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// 
		/// 
		///@target_id (uuid) ID of single target to get.
		///@tasks (boolean) Whether to include list of tasks that use the target.
		///@sort_order (sort_order)
		///@sort_field (text)
		/// </summary>
		/// <returns>
		/// A <see cref="List<OpenVASTarget>"/>
		/// </returns>
		public XmlDocument GetTargets(string targetID, bool getTasks, string sortOrder, string sortField)
		{
			CheckSession();
			
			string command = "<get_targets target_id=\"" + targetID + "\" ";
			command = command + "tasks=\"" + getTasks + "\" sort_order=\"" + sortOrder + "\" sort_field=\"" + sortField + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the tasks.
		/// </summary>
		/// <returns>
		/// The tasks.
		/// </returns>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
		/// <param name='details'>
		/// Details.
		/// </param>
		/// <param name='rcFile'>
		/// Rc file.
		/// </param>
		/// <param name='applyOverrides'>
		/// Apply overrides.
		/// </param>
		/// <param name='sortOrder'>
		/// Sort order.
		/// </param>
		/// <param name='sortField'>
		/// Sort field.
		/// </param>
		public XmlDocument GetTasks(string taskID, bool details, bool rcFile, bool applyOverrides, string sortOrder, string sortField)
		{
			CheckSession();
			
			string command = "<get_tasks ";

			if (taskID != string.Empty)
				command += "task_id=\"" + taskID + "\" ";


			command += "details=\"" + (details ? "1" : "0") + "\" rcfile=\"" + (rcFile ? "1" : "0") + 
				"\" apply_overrides=\"" + (applyOverrides ? "1" : "0") + "\"";
			
			if (!string.IsNullOrEmpty(sortOrder))
				command = command + "sort_order=\"" + sortOrder + "\"";
			
			if (!string.IsNullOrEmpty(sortField))
				command = command + "sort_field=\"" + sortField + "\"";
			
			command = command + " />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Gets the version.
		/// </summary>
		/// <returns>
		/// The version.
		/// </returns>
		public XmlDocument GetVersion()
		{
			CheckSession();
			
			string command = "<get_version />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		public XmlDocument ModifyConfig()
		{
			throw new NotImplementedException();
		}
		
		public XmlDocument ModifyLSCCredential()
		{
			throw new NotImplementedException();
		}
		
		
		public XmlDocument ModifyNote()
		{
			throw new NotImplementedException();
		}
		
		public XmlDocument ModifyOverride()
		{
			throw new NotImplementedException();
		}
		
		public XmlDocument ModifyReport()
		{
			throw new NotImplementedException();
		}
		
		public XmlDocument ModifyReportFormat()
		{
			throw new NotImplementedException();
		}
		
		public XmlDocument ModifyTask()
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Pauses the task.
		/// </summary>
		/// <returns>
		/// The task.
		/// </returns>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
		public XmlDocument PauseTask(string taskID)
		{
			CheckSession();
			
			string command = "<pause_task task_id=\"" + taskID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Resumes the or start task.
		/// </summary>
		/// <returns>
		/// The or start task.
		/// </returns>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
		public XmlDocument ResumeOrStartTask(string taskID)
		{
			CheckSession();
			
			string command = "<resume_or_start_task task_id=\"" + taskID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Resumes the paused task.
		/// </summary>
		/// <returns>
		/// The paused task.
		/// </returns>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
		public XmlDocument ResumePausedTask(string taskID)
		{
			CheckSession();
			
			string command = "<resume_paused_task task_id=\"" + taskID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Resumes the stopped task.
		/// </summary>
		/// <returns>
		/// The stopped task.
		/// </returns>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
		public XmlDocument ResumeStoppedTask(string taskID)
		{
			CheckSession();
			
			string command = "<resume_stopped_task task_id=\"" + taskID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Starts the task.
		/// </summary>
		/// <returns>
		/// The task.
		/// </returns>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
	public XmlDocument StartTask(string taskID)
		{
			CheckSession();
			
			string command = "<start_task task_id=\"" + taskID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		/// <summary>
		/// Stops the task.
		/// </summary>
		/// <returns>
		/// The task.
		/// </returns>
		/// <param name='taskID'>
		/// Task ID.
		/// </param>
		public XmlDocument StopTask(string taskID)
		{
			CheckSession();
			
			string command = "<stop_task task_id=\"" + taskID + "\" />";
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(command);
			
			XmlDocument response = _session.ExecuteCommand(doc);
			
			return response;
		}
		
		public bool TestEscalator()
		{
			throw new NotImplementedException();
		}
		
		public bool VerifyAgent()
		{
			throw new NotImplementedException();
		}
		
		public bool VerifyReportFormat()
		{
			throw new NotImplementedException();
		}
		
		private bool CheckSession()
		{
			if (!_session.Stream.CanRead)
				throw new Exception("Bad session");
			
			return true;
		}
		
		public void Dispose()
		{
		}
		
	}
}
