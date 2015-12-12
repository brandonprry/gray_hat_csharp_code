using System;
using MsgPack;
using System.Collections.Generic;

namespace ch14_automating_arachni
{
	public class ArachniManager : IDisposable
	{
		ArachniSession _session;

		public ArachniManager (ArachniSession session)
		{
			if (!session.IsInstanceStream)
				throw new Exception ("Session must be using an instance stream, not a dispatcher stream");

			_session = session;
		}

		public MessagePackObject StartScan (string url, string checks = "*")
		{
			Dictionary<string, object> args = new Dictionary<string, object> ();
			args ["url"] = url;
			args ["checks"] = checks;
			args ["audit"] = new Dictionary<string, object> ();
			((Dictionary<string, object>)args ["audit"]) ["elements"] = new object[] { "links", "forms" };

			return _session.ExecuteCommand ("service.scan", new object[]{ args }, _session.Token);
		}

		public MessagePackObject GetProgress (List<uint> digests = null)
		{
			Dictionary<string, object> args = new Dictionary<string, object> ();
			args ["with"] = "issues";
			if (digests != null) {
				args ["without"] = new Dictionary<string, object> ();
				((Dictionary<string, object>)args ["without"]) ["issues"] = digests.ToArray ();
			}
			return _session.ExecuteCommand ("service.progress", new object[] { args }, _session.Token);
		}

		public MessagePackObject IsBusy ()
		{
			return _session.ExecuteCommand ("service.busy?", new object[]{ }, _session.Token);
		}

		public void Dispose ()
		{
			_session.Dispose ();
		}
	}
}

