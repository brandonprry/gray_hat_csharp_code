using System;
using System.Net.Security;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.IO;
using System.IO.Compression;
using MsgPack.Serialization;
using MsgPack;
using zlib;

namespace ch14_automating_arachni
{
	public class ArachniSession : IDisposable
	{
		SslStream _stream = null;

		public ArachniSession (string host, int port, bool initiateInstance = false)
		{
			this.Host = host;
			this.Port = port;
			GetDispatcherStream ();
			this.IsInstanceStream = false;

			if (initiateInstance) {
				this.InstanceName = Guid.NewGuid ().ToString ();
				MessagePackObjectDictionary resp = this.ExecuteCommand ("dispatcher.dispatch", new object[] { this.InstanceName }).AsDictionary ();

				string[] url = resp ["url"].AsString ().Split (':');

				this.InstanceHost = url [0];
				this.InstancePort = int.Parse (url [1]);
				this.Token = resp ["token"].AsString ();

				GetInstanceStream ();

				bool aliveResp = this.ExecuteCommand ("service.alive?", new object[]{ }, this.Token).AsBoolean ();

				this.IsInstanceStream = aliveResp;
			}
		}

		public string Host { get; set; }

		public int Port { get; set; }

		public string Token { get; set; }

		public bool IsInstanceStream { get; set; }

		public string InstanceHost { get; set; }

		public int InstancePort { get; set; }

		public string InstanceName { get; set; }

		public MessagePackObject ExecuteCommand (string command, object[] args, string token = null)
		{
			Dictionary<string, object> message = new Dictionary<string, object> ();
			message ["message"] = command;
			message ["args"] = args;

			if (token != null)
				message ["token"] = token;

			byte[] packed;
			using (MemoryStream stream = new MemoryStream ()) {
				Packer packer = Packer.Create (stream);
				packer.PackMap (message);
				packed = stream.ToArray ();
			}
		
			byte[] packedLength = BitConverter.GetBytes (packed.Length);

			if (BitConverter.IsLittleEndian)
				Array.Reverse (packedLength);

			_stream.Write (packedLength);
			_stream.Write (packed);

			byte[] respBytes = ReadMessage (_stream);

			MessagePackObjectDictionary resp = null;
			try {
				resp = Unpacking.UnpackObject (respBytes).Value.AsDictionary ();
			} catch {
				byte[] decompressed = DecompressData (respBytes);
				resp = Unpacking.UnpackObject (decompressed).Value.AsDictionary ();
			}
			
			return resp.ContainsKey ("obj") ? resp ["obj"] : resp ["exception"];
		}

		public byte[] DecompressData (byte[] inData)
		{
			using (MemoryStream outMemoryStream = new MemoryStream ()) {
				using (ZOutputStream outZStream = new ZOutputStream (outMemoryStream)) {
					outZStream.Write (inData, 0, inData.Length);
					return outMemoryStream.ToArray ();
				}
			}
		}

		private byte[] ReadMessage (SslStream sslStream)
		{
			byte[] sizeBytes = new byte[4];
			sslStream.Read (sizeBytes, 0, sizeBytes.Length);

			if (BitConverter.IsLittleEndian)
				Array.Reverse (sizeBytes);

			uint size = BitConverter.ToUInt32 (sizeBytes, 0);

			byte[] buffer = new byte[size];
			sslStream.Read (buffer, 0, buffer.Length);

			return buffer;
		}

		private void GetDispatcherStream ()
		{
			TcpClient client = new TcpClient (this.Host, this.Port);

			_stream = new SslStream (client.GetStream (), false, new RemoteCertificateValidationCallback (ValidateServerCertificate), 
				(sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) => null);

			_stream.AuthenticateAsClient ("arachni", null, SslProtocols.Tls, false);
		}

		private void GetInstanceStream ()
		{
			TcpClient client = new TcpClient (this.InstanceHost, this.InstancePort);

			_stream = new SslStream (client.GetStream (), false, new RemoteCertificateValidationCallback (ValidateServerCertificate), 
				(sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) => null);

			_stream.AuthenticateAsClient ("arachni", null, SslProtocols.Tls, false);
		}

		private bool ValidateServerCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public void Dispose ()
		{
			if (this.IsInstanceStream && _stream != null)
				this.ExecuteCommand ("service.shutdown", new object[] { }, this.Token);
			
			_stream = null;
		}
	}
}

