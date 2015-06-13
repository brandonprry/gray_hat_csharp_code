using System;
using System.Xml;

namespace nexposesharp
{
	public class NexposeManager : IDisposable
	{
		private NexposeSession _session;
		
		public NexposeManager (NexposeSession session)
		{
			if (!session.IsAuthenticated)
				throw new Exception("Trying to create manager from unauthenticated session. Please authenticate.");
			
			_session = session;
		}

		public void Dispose()
		{
			_session.Logout();
		}
	}
}

