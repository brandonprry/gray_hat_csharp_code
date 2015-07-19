using System;
using Newtonsoft.Json.Linq;

namespace ch6_automating_nessus
{
	public class NessusManager : IDisposable
	{
		NessusSession _session;
		public NessusManager (NessusSession session){
			_session = session;
		}

		public JObject GetScanPolicies(){
			return _session.MakeRequest("GET", "/editor/policy/templates", null, _session.Token);
		}

		public JObject CreateScan(string policyID, string cidr, string name, string description){
			JObject data = new JObject();
			data ["uuid"] = policyID;
			data ["settings"] = new JObject ();
			data ["settings"] ["name"] = name;
			data ["settings"] ["text_targets"] = cidr;
			data ["settings"] ["description"] = description;

			return _session.MakeRequest ("POST", "/scans", data, _session.Token);
		}

		public JObject StartScan(int scanID) {
			return _session.MakeRequest ("POST", "/scans/" + scanID + "/launch", null, _session.Token);
		}

		public JObject GetScan(int scanID) {
			return _session.MakeRequest ("GET", "/scans/" + scanID, null, _session.Token);
		}

		public void Dispose()
		{
			if (_session.Authenticated) {
				_session.LogOut ();
			}
			
			_session = null;
		}
	}
}

