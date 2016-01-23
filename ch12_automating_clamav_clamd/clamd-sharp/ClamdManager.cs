using System;

namespace clamdsharp
{
	public class ClamdManager
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

		public string Scan (string path)
		{
			return _session.Execute("SCAN " + path);
		}
	}
}

