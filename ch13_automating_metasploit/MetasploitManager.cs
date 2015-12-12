using System;
using System.Collections.Generic;

namespace ch13_automating_metasploit
{
	public class MetasploitManager : IDisposable
	{
		private MetasploitSession _session;

		public MetasploitManager (MetasploitSession session)
		{
			_session = session;
		}

		public Dictionary<string, object> GetCoreModuleStats ()
		{
			return _session.Execute ("core.module_stats");
		}

		public Dictionary<string, object> GetCoreVersionInformation ()
		{
			return _session.Execute ("core.version");
		}

		public Dictionary<string, object> ListJobs ()
		{
			return _session.Execute ("job.list");
		}

		public Dictionary<string, object> StopJob (string jobID)
		{
			return _session.Execute ("job.stop", jobID);
		}

		public Dictionary<string, object> ExecuteModule (string moduleType, string moduleName, Dictionary<string, object> options)
		{
			return _session.Execute ("module.execute", moduleType, moduleName, options);
		}

		public Dictionary<string, object> ListSessions ()
		{
			return _session.Execute ("session.list");
		}

		public Dictionary<string, object> StopSession (string sessionID)
		{
			return _session.Execute ("session.stop", sessionID);
		}

		public Dictionary<string, object> ReadSessionShell (string sessionID)
		{
			return this.ReadSessionShell (sessionID, null);
		}

		public Dictionary<string, object> ReadSessionShell (string sessionID, int? readPointer)
		{
			if (readPointer.HasValue)
				return _session.Execute ("session.shell_read", sessionID, readPointer.Value);
			else
				return _session.Execute ("session.shell_read", sessionID);
		}

		public Dictionary<string, object> WriteToSessionShell (string sessionID, string data)
		{
			return _session.Execute ("session.shell_write", sessionID, data);
		}

		public void Dispose ()
		{
			_session = null;
		}
	}
}

