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

		public Dictionary<string, object> AddCoreModulePath (string modulePath)
		{
			return _session.Execute ("core.add_module_path", modulePath);
		}

		public Dictionary<string, object> ReloadCoreModules ()
		{
			return _session.Execute ("core.reload_modules");
		}

		public Dictionary<string, object> SaveCore ()
		{
			return _session.Execute ("core.save");
		}

		public Dictionary<string,object> SetCoreGlobalVariable (string optionName, string optionValue)
		{
			return _session.Execute ("core.setg", optionName, optionValue);
		}

		public Dictionary<string, object> UnsetCoreGlobalVariable (string optionName)
		{
			return _session.Execute ("core.unsetg", optionName);
		}

		public Dictionary<string, object> GetCoreThreadList ()
		{
			return _session.Execute ("core.thread_list");
		}

		public Dictionary<string, object> KillCoreThread (string threadID)
		{
			return _session.Execute ("core.thread_kill", threadID);
		}

		public Dictionary<string, object> StopCore ()
		{
			return _session.Execute ("core.stop");
		}

		public Dictionary<string, object> CreateConsole ()
		{
			return _session.Execute ("console.create");
		}

		public Dictionary<string, object> DestroyConsole (string consoleID)
		{
			return _session.Execute ("console.destroy", consoleID);
		}

		public Dictionary<string, object> ListConsoles ()
		{
			return _session.Execute ("console.list");
		}

		public Dictionary<string, object> WriteToConsole (string consoleID, string data)
		{
			return _session.Execute ("console.write", consoleID, data);
		}

		public Dictionary<string, object> ReadConsole (string consoleID)
		{
			return _session.Execute ("console.read", consoleID);
		}

		public Dictionary<string, object> DetachSessionFromConsole (string consoleID)
		{
			return _session.Execute ("console.session_detach", consoleID);
		}

		public Dictionary<string, object> KillSessionFromConsole (string consoleID)
		{
			return _session.Execute ("console.session_kill", consoleID);
		}

		public Dictionary<string, object> TabConsole (string consoleID, string input)
		{
			return _session.Execute ("console.tabs", consoleID, input);
		}

		public Dictionary<string, object> ListJobs ()
		{
			return _session.Execute ("job.list");
		}

		public Dictionary<string, object> GetJobInfo (string jobID)
		{
			return _session.Execute ("job.info", jobID);
		}

		public Dictionary<string, object> StopJob (string jobID)
		{
			return _session.Execute ("job.stop", jobID);
		}

		public Dictionary<string, object> GetExploitModules ()
		{
			return _session.Execute ("module.exploits");
		}

		public Dictionary<string, object> GetAuxiliaryModules ()
		{
			return _session.Execute ("module.auxiliary");
		}

		public Dictionary<string, object> GetPostModules ()
		{
			return _session.Execute ("module.post");
		}

		public Dictionary<string, object> GetPayloads ()
		{
			return _session.Execute ("module.payloads");
		}

		public Dictionary<string, object> GetEncoders ()
		{
			return _session.Execute ("module.encoders");
		}

		public Dictionary<string, object> GetNops ()
		{
			return _session.Execute ("module.nops");
		}

		public Dictionary<string, object> GetModuleInformation (string moduleType, string moduleName)
		{
			return _session.Execute ("module.info", moduleType, moduleName);
		}

		public Dictionary<string, object> GetModuleOptions (string moduleType, string moduleName)
		{
			return _session.Execute ("module.options", moduleType, moduleName);
		}

		public Dictionary<string, object> GetModuleCompatiblePayloads (string moduleName)
		{
			return _session.Execute ("module.compatible_payloads", moduleName);
		}

		public Dictionary<string, object> GetModuleTargetCompatiblePayloads (string moduleName, int targetIndex)
		{
			return _session.Execute ("module.target_compatible_payloads", moduleName, targetIndex);
		}

		public Dictionary<string, object> GetModuleCompatibleSessions (string moduleName)
		{
			return _session.Execute ("module.compatible_sessions", moduleName);
		}

		public Dictionary<string, object> Encode (string data, string encoderModule, Dictionary<string, object> options)
		{
			return _session.Execute ("module.encode", data, encoderModule, options);
		}

		public Dictionary<string, object> ExecuteModule (string moduleType, string moduleName, Dictionary<string, object> options)
		{
			return _session.Execute ("module.execute", moduleType, moduleName, options);
		}

		public Dictionary<string, object> LoadPlugin (string pluginName, Dictionary<string, object> options)
		{
			return _session.Execute ("plugin.load", pluginName, options);
		}

		public Dictionary<string, object> UnloadPlugin (string pluginName)
		{
			return _session.Execute ("plugin.unload", pluginName);
		}

		public Dictionary<string, object> ListLoadedPlugins ()
		{
			return _session.Execute ("plugin.loaded");
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

		public Dictionary<string, object> WriteToSessionMeterpreter (string sessionID, string data)
		{
			return _session.Execute ("session.meterpreter_write", sessionID, data);
		}

		public Dictionary<string, object> ReadSessionMeterpreter (string sessionID)
		{
			return _session.Execute ("session.meterpreter_read", sessionID);
		}

		public Dictionary<string, object> RunSessionMeterpreterSingleCommand (string sessionID, string command)
		{
			return _session.Execute ("session.meterpreter_run_single", sessionID, command);
		}

		public Dictionary<string, object> RunSessionMeterpreterScript (string sessionID, string scriptName)
		{
			return _session.Execute ("session.meterpreter_script", sessionID, scriptName);
		}

		public Dictionary<string, object> DetachMeterpreterSession (string sessionID)
		{
			return _session.Execute ("session.meterpreter_session_detach", sessionID);
		}

		public Dictionary<string, object> KillMeterpreterSession (string sessionID)
		{
			return _session.Execute ("session.meterpreter_session_kill", sessionID);
		}

		public Dictionary<string, object> TabMeterpreterSession (string sessionID, string input)
		{
			return _session.Execute ("session.meterpreter_tabs", sessionID, input);
		}

		public Dictionary<string, object> CompatibleModuleForSession (string sessionID)
		{
			return _session.Execute ("session.compatible_modules", sessionID);
		}

		public Dictionary<string, object> UpgradeShellToMeterpreter (string sessionID, string host, string port)
		{
			return _session.Execute ("session.shell_upgrade", sessionID, host, port);
		}

		public Dictionary<string, object> ClearSessionRing (string sessionID)
		{
			return _session.Execute ("session.ring_clear", sessionID);
		}

		public Dictionary<string, object> LastSessionRing (string sessionID)
		{
			return _session.Execute ("session.ring_last", sessionID);
		}

		public Dictionary<string, object> WriteToSessionRing (string sessionID, string data)
		{
			return _session.Execute ("session.ring_put", sessionID, data);
		}

		public Dictionary<string, object> ReadSessionRing (string sessionID, int? readPointer)
		{
			if (readPointer.HasValue)
				return _session.Execute ("session.ring_read", sessionID, readPointer.Value);
			else
				return _session.Execute ("session.ring_read", sessionID);
		}

		
		
		public void Dispose ()
		{
		}
	}
}

