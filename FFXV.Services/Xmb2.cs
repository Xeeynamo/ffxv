using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXV.Services
{
	public class Xmb2
	{
		internal class Entry
		{
			public int Offset { get; set; }

			public int Unk04 { get; set; }

			public int Unk08 { get; set; }
		}

		public Xmb2(BinaryReader reader)
		{
			var magicCode = reader.ReadUInt32();
			var endOffset = reader.ReadInt32();

			var unk08Offset = reader.ReadInt32();
			var unk0cOffset = reader.ReadInt32();
			var unk10Offset = reader.ReadInt32();


		}
	}
}
