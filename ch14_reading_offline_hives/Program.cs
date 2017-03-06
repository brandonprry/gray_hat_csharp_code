using System;
using System.Linq;
using ntregsharp;
using System.Collections.Generic;
using System.Text;

namespace ch11_reading_offline_hives
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			RegistryHive systemHive = new RegistryHive (args [0]);
			RegistryHive samHive = new RegistryHive(args[1]);
			RegistryHive softwareHive = new RegistryHive(args[2]);

			byte[] bootKey = GetBootKey (systemHive);
			Console.WriteLine ("Boot key: " + BitConverter.ToString (bootKey));

			ListSystemUsers(samHive);
			ListInstalledSoftware(softwareHive);

			Console.WriteLine("Dumping hive information complete");
		}

		static void ListSystemUsers(RegistryHive samHive)
		{
			NodeKey key = GetNodeKey(samHive, "SAM\\Domains\\Account\\Users\\Names");

			foreach (NodeKey child in key.ChildNodes)
				Console.WriteLine(child.Name);
		}

		static void ListInstalledSoftware(RegistryHive softwareHive)
		{
			NodeKey key = GetNodeKey(softwareHive, "Microsoft\\Windows\\CurrentVersion\\Uninstall");

			foreach (NodeKey child in key.ChildNodes)
			{
				Console.WriteLine("Found: " + child.Name);
				ValueKey val = child.ChildValues.SingleOrDefault(v => v.Name == "DisplayVersion");

				if (val != null)
				{
					string version = System.Text.Encoding.UTF8.GetString(val.Data);
					Console.WriteLine("\tVersion: " + version);
				}

				val = child.ChildValues.SingleOrDefault(v => v.Name == "InstallLocation");

				if (val != null)
				{
					string location = System.Text.Encoding.UTF8.GetString(val.Data);
					Console.WriteLine("\tLocation: " + location);
				}

				Console.WriteLine("----");
			}

		}

		static byte[] GetBootKey(RegistryHive systemHive){
			ValueKey controlSet = GetValueKey (systemHive, "Select\\Default");
			int cs = BitConverter.ToInt32 (controlSet.Data, 0);

			StringBuilder scrambledKey = new StringBuilder ();
			foreach (string key in new string[] {"JD", "Skew1", "GBG", "Data"}) {
				NodeKey nk = GetNodeKey (systemHive, "ControlSet00" + cs + "\\Control\\Lsa\\" + key);

				for (int i = 0; i < nk.ClassnameLength && i < 8; i++) 
					scrambledKey.Append ((char)nk.ClassnameData [i*2]);
			}
				
			byte[] skey = StringToByteArray (scrambledKey.ToString ());
			byte[] descramble = new byte[] { 0x8, 0x5, 0x4, 0x2, 0xb, 0x9, 0xd, 0x3,
										     0x0, 0x6, 0x1, 0xc, 0xe, 0xa, 0xf, 0x7 };

			byte[] bootkey = new byte[16];
			for (int i = 0; i < bootkey.Length; i++)
				bootkey[i] = skey [descramble [i]];

			return bootkey;
		}

		static byte[] StringToByteArray(string hex) {
			return Enumerable.Range(0, hex.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
				.ToArray();
		}

		static NodeKey GetNodeKey(RegistryHive hive, string path){

			NodeKey node = null;
			string[] paths = path.Split ('\\');

			for (int i = 0;i < paths.Length; i++) {

				if (node == null)
					node = hive.RootKey;
				
				foreach (NodeKey child in node.ChildNodes) {
					if (child.Name == paths [i]) {
						node = child;
						break;
					}
				}

				throw new Exception("No child node found with name " + paths[i]);
			}

			return node;
		}

		static ValueKey GetValueKey(RegistryHive hive, string path) {

			string keyname = path.Split ('\\').Last ();
			NodeKey node = GetNodeKey (hive, path);

			return node.ChildValues.SingleOrDefault (v => v.Name == keyname);
		}
	}
}
