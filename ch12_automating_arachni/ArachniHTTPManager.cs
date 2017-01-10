using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ch14_automating_arachni
{
	public class ArachniHTTPManager
	{
		ArachniHTTPSession _session;

		public ArachniHTTPManager (ArachniHTTPSession session)
		{
			_session = session;
		}

		public JObject StartScan(string url, JObject options = null) {
			JObject data = new JObject ();
			data ["url"] = url;
			data.Merge (options);

			return _session.ExecuteRequest ("POST", "/scans", data);
		}

		public JObject GetScanStatus(Guid id){
			return _session.ExecuteRequest ("GET", "/scans/" + id.ToString ().Replace("-", string.Empty));
		}
	}
}

