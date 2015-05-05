using System;

namespace clamdsharp
{
	public class ClamdManager : IDisposable
	{
		private ClamdSession _session = null;

		public ClamdManager (ClamdSession session)
		{
			_session = session;
		}

		public string GetVersion ()
		{
			return _session.Execute("VERSION");
		}

		public string GetStatistics ()
		{
			return _session.Execute("STATS");
		}

		public string Ping()
		{
			return _session.Execute("PING");
		}

		public string ScanWithArchiveSupport (string path)
		{
			return _session.Execute("SCAN " + path);
		}

		public string ScanWithoutArchiveSupport (string path)
		{
			return _session.Execute("RAWSCAN " + path);
		}

		public void Shutdown ()
		{
			_session.Execute("SHUTDOWN");
		}

		public string ReloadDatabase ()
		{
			return _session.Execute("RELOAD");
		}

		public string ContinuousScanWithArchiveSupport (string path)
		{
			return _session.Execute("CONTSCAN " + path);
		}

		public string MultithreadScan (string path)
		{
			return _session.Execute("MULTISCAN " + path);
		}

		public void Dispose() {}
	}
}

