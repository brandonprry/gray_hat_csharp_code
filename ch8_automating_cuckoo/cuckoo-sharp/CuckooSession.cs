using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace cuckoosharp
{
	public class CuckooSession
	{
		public CuckooSession (string host, int port)
		{
			this.Host = host;
			this.Port = port;
		}
		
		public string Host { get; set; }
		public int Port { get; set; }
		
		public JObject ExecuteCommand (string uri, string verb)
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create ("http://" + this.Host + ":" + this.Port + uri);
			req.Method = verb;
			
			string resp = string.Empty;
			using (Stream str = req.GetResponse().GetResponseStream())
				using (StreamReader rdr = new StreamReader(str))
					resp = rdr.ReadToEnd ();

			JObject obj = JObject.Parse(resp);

			return obj;
		}
		
		public JObject ExecuteCommand (string uri, string verb, IDictionary<string, object> parms)
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create ("http://" + this.Host + ":" + this.Port + uri);
			req.Method = verb;
			string boundary = String.Format("----------{0:N}", Guid.NewGuid());
			byte[] data = GetMultipartFormData(parms, boundary);
			
			req.ContentLength = data.Length;
			req.ContentType = "multipart/form-data; boundary=" + boundary;
			
			using (Stream pstream = req.GetRequestStream())
				pstream.Write (data, 0, data.Length);
			
			string resp = string.Empty;
			using (Stream str = req.GetResponse().GetResponseStream())
				using (StreamReader rdr = new StreamReader(str))
					resp = rdr.ReadToEnd ();
			
			JObject obj = JObject.Parse(resp);
			return obj;
		}
		
		
		//got this method from http://www.briangrinstead.com/blog/multipart-form-post-in-c
		private byte[] GetMultipartFormData (IDictionary<string, object> postParameters, string boundary)
		{
			Stream formDataStream = new System.IO.MemoryStream ();
			bool needsCLRF = false;
 
			foreach (var param in postParameters) {
				// Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
				// Skip it on the first parameter, add it to subsequent parameters.
				if (needsCLRF)
					formDataStream.Write (encoding.GetBytes ("\r\n"), 0, encoding.GetByteCount ("\r\n"));
 
				needsCLRF = true;
 
				if (param.Value is FileParameter) {
					FileParameter fileToUpload = (FileParameter)param.Value;
 
					// Add just the first part of this param, since we will write the file data directly to the Stream
					string header = string.Format ("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                    boundary,
                    param.Key,
                    fileToUpload.FileName ?? param.Key,
                    fileToUpload.ContentType ?? "application/octet-stream");
 
					formDataStream.Write (encoding.GetBytes (header), 0, encoding.GetByteCount (header));
 
					// Write the file data directly to the Stream, rather than serializing it to a string.
					formDataStream.Write (fileToUpload.File, 0, fileToUpload.File.Length);
				} else {
					string postData = string.Format ("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    boundary,
                    param.Key,
                    param.Value);
					formDataStream.Write (encoding.GetBytes (postData), 0, encoding.GetByteCount (postData));
				}
			}
 
			// Add the end of the request.  Start with a newline
			string footer = "\r\n--" + boundary + "--\r\n";
			formDataStream.Write (encoding.GetBytes (footer), 0, encoding.GetByteCount (footer));
 
			// Dump the Stream into a byte[]
			formDataStream.Position = 0;
			byte[] formData = new byte[formDataStream.Length];
			formDataStream.Read (formData, 0, formData.Length);
			formDataStream.Close ();
 
			return formData;
		}
		
		private System.Text.Encoding encoding = System.Text.Encoding.ASCII;
	}
	
	public class FileParameter
	{
		public byte[] File { get; set; }
		public string FileName { get; set; }
		public string ContentType { get; set; }

		public FileParameter (byte[] file, string filename, string contenttype)
		{
			File = file;
			FileName = filename;
			ContentType = contenttype;
		}
	}
}

