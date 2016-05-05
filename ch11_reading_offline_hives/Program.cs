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
			byte[] bootKey = GetBootKey (systemHive);

			Console.WriteLine ("Boot key: " + BitConverter.ToString (bootKey));
		}

		static byte[] GetBootKey(RegistryHive hive){
			ValueKey controlSet = GetValueKey (hive, "Select|Default");
			int cs = BitConverter.ToInt32 (controlSet.Data, 0);

			StringBuilder scrambledKey = new StringBuilder ();
			foreach (string key in new string[] {"JD", "Skew1", "GBG", "Data"}) {
				NodeKey nk = GetNodeKey (hive, "ControlSet00" + cs + "|Control|Lsa|" + key);

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
			string[] paths = path.Split ('|');

			for (int i = 0;i < paths.Length; i++) {

				if (node == null)
					node = hive.RootKey;
				
				foreach (NodeKey child in node.ChildNodes) {
					if (child.Name == paths [i]) {
						node = child;
						break;
					}
				}
			}

			return node;
		}

		static ValueKey GetValueKey(RegistryHive hive, string path) {

			string keyname = path.Split ('|').Last ();
			NodeKey node = GetNodeKey (hive, path);

			return node.ChildValues.SingleOrDefault (v => v.Name == keyname);
		}
	}
}
