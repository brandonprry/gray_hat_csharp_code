using System;
using openvassharp;
using System.Xml;

namespace ch8_automating_openvas
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (OpenVASManagerSession session = new OpenVASManagerSession("admin", "password", "192.168.1.99"))
			{
				using (OpenVASManager manager = new OpenVASManager(session))
				{
					XmlDocument version = manager.GetVersion ();
					Console.WriteLine (version.OuterXml);
				}
			}
		}
	}
}
