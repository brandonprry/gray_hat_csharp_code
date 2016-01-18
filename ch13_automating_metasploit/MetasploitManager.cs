using System;
using System.Collections.Generic;
using MsgPack;

namespace ch13_automating_metasploit
{
	public class MetasploitManager : IDisposable
	{
		private MetasploitSession _session;

		public MetasploitManager (MetasploitSession session)
		{
			_session = session;
		}

		public Dictionary<object, object> GetCoreModuleStats ()
		{
			return _session.Execute ("core.module_stats");
		}

		public Dictionary<object, object> GetCoreVersionInformation ()
		{
			return _session.Execute ("core.version");
		}

		public Dictionary<object, object> ListJobs ()
		{
			return _session.Execute ("job.list");
		}

		public Dictionary<object, object> StopJob (string jobID)
		{
			return _session.Execute ("job.stop", jobID);
		}

		public Dictionary<object, object> ExecuteModule (string moduleType, string moduleName, Dictionary<object, object> options)
		{
			return _session.Execute ("module.execute", moduleType, moduleName, options);
		}

		public Dictionary<object, object> ListSessions ()
		{
			return _session.Execute ("session.list");
		}

		public Dictionary<object, object> StopSession (string sessionID)
		{
			return _session.Execute ("session.stop", sessionID);
		}

		public Dictionary<object, object> ReadSessionShell (string sessionID)
		{
			return this.ReadSessionShell (sessionID, null);
		}

		public Dictionary<object, object> ReadSessionShell (string sessionID, int? readPointer)
		{
			if (readPointer.HasValue)
				return _session.Execute ("session.shell_read", sessionID, readPointer.Value);
			else
				return _session.Execute ("session.shell_read", sessionID);
		}

		public Dictionary<object, object> WriteToSessionShell (string sessionID, string data)
		{
			return _session.Execute ("session.shell_write", sessionID, data);
		}

		public void Dispose ()
		{
			_session = null;
		}
	}
}

