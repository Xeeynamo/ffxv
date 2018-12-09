using System.Collections.Generic;
using System.IO;
using FFXV.Utilities;

namespace FFXV.Services
{
	public class Msg
	{
		public static Dictionary<int, string> Open(Stream stream)
		{
			var reader = new BinaryReader(stream);
			stream.Position = 0x100; // horrible hack, which skip the first header

			if (reader.ReadInt32() != 0x76656442 ||
			    reader.ReadInt32() != 0x6F736552 ||
			    reader.ReadInt32() != 0x65637275)
			{
				throw new InvalidDataException("Not a BdevResource file");
			}

			var totalLength = reader.ReadInt32(); // 0x247F2
			var unk10 = reader.ReadInt32(); //0x40B9E, fixed?
			var unk14 = reader.ReadInt32(); //0x4AC71, fixed?
			var unk18 = reader.ReadInt32(); //0x403F25A, variable... unknown.
			var unk1c = reader.ReadInt32(); //0x0, fixed?

			var basePosition = stream.Position;
			var msgCount = reader.ReadInt32();

			var msgs = new Dictionary<int, string>(msgCount);
			var msgOffsets = new int[msgCount];

			for (var i = 0; i < msgCount; i++)
			{
				var id = reader.ReadInt32();
				var offset = basePosition + reader.ReadInt32();
				var position = stream.Position;

				stream.Position = offset & 0x3FFFF;
				msgs[id] = reader.ReadCString();
				stream.Position = position;
			}

			return msgs;
		}
	}
}
