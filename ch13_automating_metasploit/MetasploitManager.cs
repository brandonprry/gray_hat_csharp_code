using System;
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

		public MessagePackObjectDictionary GetCoreModuleStats ()
		{
			return _session.Execute ("core.module_stats");
		}

		public MessagePackObjectDictionary GetCoreVersionInformation ()
		{
			return _session.Execute ("core.version");
		}

		public MessagePackObjectDictionary ListJobs ()
		{
			return _session.Execute ("job.list");
		}

		public MessagePackObjectDictionary StopJob (string jobID)
		{
			return _session.Execute ("job.stop", jobID);
		}

		public MessagePackObjectDictionary ExecuteModule (string moduleType, string moduleName, MessagePackObjectDictionary options)
		{
			return _session.Execute ("module.execute", moduleType, moduleName, options);
		}

		public MessagePackObjectDictionary ListSessions ()
		{
			return _session.Execute ("session.list");
		}

		public MessagePackObjectDictionary StopSession (string sessionID)
		{
			return _session.Execute ("session.stop", sessionID);
		}

		public MessagePackObjectDictionary ReadSessionShell (string sessionID)
		{
			return this.ReadSessionShell (sessionID, null);
		}

		public MessagePackObjectDictionary ReadSessionShell (string sessionID, int? readPointer)
		{
			if (readPointer.HasValue)
				return _session.Execute ("session.shell_read", sessionID, readPointer.Value);
			else
				return _session.Execute ("session.shell_read", sessionID);
		}

		public MessagePackObjectDictionary WriteToSessionShell (string sessionID, string data)
		{
			return _session.Execute ("session.shell_write", sessionID, data);
		}

		public void Dispose ()
		{
			_session = null;
		}
	}
}

