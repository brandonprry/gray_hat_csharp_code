using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace ntregsharp
{
	public class NodeKey
	{
		public NodeKey (BinaryReader hive)
		{
			ReadNodeStructure (hive);
			ReadChildrenNodes (hive);
			ReadChildValues (hive);
		}

		private void ReadNodeStructure(BinaryReader hive) {
			byte[] buf = hive.ReadBytes(4);

			if (buf[0] != 110 || buf[1] != 107)
				throw new NotSupportedException("Bad nk header");

			long startingOffset = hive.BaseStream.Position;
			this.IsRootKey = (buf[2] == 0x2c) ? true : false;

			this.Timestamp = DateTime.FromFileTime(BitConverter.ToInt64(hive.ReadBytes(8), 0));

			hive.BaseStream.Position += 4; 

			this.ParentOffset = BitConverter.ToInt32(hive.ReadBytes (4),0);
			this.SubkeysCount = BitConverter.ToInt32(hive.ReadBytes (4),0);

			hive.BaseStream.Position += 4;

			this.LFRecordOffset = BitConverter.ToInt32(hive.ReadBytes (4),0);

			hive.BaseStream.Position += 4;

			this.ValuesCount = BitConverter.ToInt32(hive.ReadBytes (4),0);
			this.ValueListOffset = BitConverter.ToInt32(hive.ReadBytes (4),0);
			this.SecurityKeyOffset = BitConverter.ToInt32(hive.ReadBytes (4),0);
			this.ClassnameOffset = BitConverter.ToInt32(hive.ReadBytes (4),0);

			hive.BaseStream.Position += (startingOffset + 0x0044) - hive.BaseStream.Position;

			this.NameLength = BitConverter.ToInt16(hive.ReadBytes (2),0);
			this.ClassnameLength = BitConverter.ToInt16(hive.ReadBytes (2),0);

			buf = hive.ReadBytes(this.NameLength);
			this.Name = System.Text.Encoding.UTF8.GetString(buf);
		}

		private void ReadChildrenNodes(BinaryReader hive) {
			this.ChildNodes = new List<NodeKey> ();
			if (this.LFRecordOffset != -1)
			{
				hive.BaseStream.Position = 4096 + this.LFRecordOffset + 4;

				byte[] buf = hive.ReadBytes(2);

				//ri
				if (buf [0] == 0x72 && buf [1] == 0x69) {
					int count = BitConverter.ToInt16(hive.ReadBytes(2),0);

					for (int i = 0; i < count; i++) {
						long pos = hive.BaseStream.Position;
						int offset = BitConverter.ToInt32 (hive.ReadBytes (4), 0);
						hive.BaseStream.Position =  4096 + offset + 4;
						buf = hive.ReadBytes(2);

						if (!(buf [0] == 0x6c && (buf [1] == 0x66 || buf [1] == 0x68))) 
							throw new Exception ("Bad LF/LH record at: " + hive.BaseStream.Position);

						ParseChildNodes (hive);

						hive.BaseStream.Position = pos + 4; //go to next record list
					}
				}
				//lf or lh
				else if (buf[0] == 0x6c && (buf[1] == 0x66 || buf[1] == 0x68))
					ParseChildNodes (hive);
				else
					throw new Exception("Bad LF/LH/RI Record at: " + hive.BaseStream.Position);
			}
		}

		private void ParseChildNodes(BinaryReader hive){
			int count = BitConverter.ToInt16(hive.ReadBytes(2),0);
			long topOfList = hive.BaseStream.Position;

			for (int i = 0; i < count; i++)
			{
				hive.BaseStream.Position = topOfList + (i*8);
				int newoffset = BitConverter.ToInt32(hive.ReadBytes(4),0);
				hive.BaseStream.Position += 4;
				//byte[] check = hive.ReadBytes(4);
				hive.BaseStream.Position = 4096 + newoffset + 4;
				NodeKey nk = new NodeKey (hive) { ParentNodeKey = this };
				this.ChildNodes.Add(nk);
			}

			hive.BaseStream.Position = topOfList + (count * 8);
		}

		private void ReadChildValues(BinaryReader hive) { 
			this.ChildValues = new List<ValueKey>();
			if (this.ValueListOffset != -1)
			{
				hive.BaseStream.Position = 4096 + this.ValueListOffset + 4;

				for (int i = 0; i < this.ValuesCount; i++)
				{
					hive.BaseStream.Position = 4096 + this.ValueListOffset + 4 + (i*4);
					int offset = BitConverter.ToInt32(hive.ReadBytes(4), 0);
					hive.BaseStream.Position = 4096 + offset + 4;
					this.ChildValues.Add(new ValueKey(hive));
				}
			}
		}

		public List<NodeKey> ChildNodes { get; set; }
		public List<ValueKey> ChildValues { get; set; }
		public DateTime Timestamp { get; set; }
		public int ParentOffset { get; set; }
		public int SubkeysCount { get; set; }
		public int LFRecordOffset { get; set; }
		public int ClassnameOffset { get; set; }
		public int SecurityKeyOffset { get; set; }
		public int ValuesCount { get; set; }
		public int ValueListOffset { get; set; }
		public short NameLength { get; set; }
		public bool IsRootKey { get; set; }
		public short ClassnameLength { get; set; }
		public string Name { get; set; }
		public byte[] ClassnameData { get; set; }
		public NodeKey ParentNodeKey { get; set; }
	}
}