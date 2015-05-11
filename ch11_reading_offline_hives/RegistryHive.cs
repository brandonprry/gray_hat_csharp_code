using System;
using System.IO;

namespace ntregsharp
{
	public class RegistryHive
	{
		public RegistryHive(string file)
		{
			if (!File.Exists(file))
				throw new FileNotFoundException();
			
			this.Filepath = file;
			
			using (FileStream stream = File.OpenRead(file))
			{
				using (BinaryReader reader = new BinaryReader(stream))
				{
					byte[] buf = reader.ReadBytes(4);
					
					if (buf[0] != 'r' || buf[1] != 'e' || buf[2] != 'g' || buf[3] != 'f')
						throw new NotSupportedException();
					
					reader.ReadBytes(8);
					buf = reader.ReadBytes(8);
					
					long timestamp = BitConverter.ToInt64(buf, 0);
					this.WasExported = (timestamp == 0) ? true : false;
					
					//fast-forward
					reader.BaseStream.Position += (0x1000 + 0x20 + 4)-reader.BaseStream.Position;
					
					this.RootKey = new NodeKey(reader);
				}
			}
		}
		
		public string Filepath { get; set; }
		
		public NodeKey RootKey { get; set; }
				
		public bool WasExported { get; set; }
	}
}

