using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXV.Utilities
{
	public static class BinaryReaderExtensions
	{
		public static int ReadOffset(this BinaryReader reader, int offset)
		{
			reader.BaseStream.Position = offset;
			return reader.ReadOffset();
		}

		public static int ReadOffset(this BinaryReader reader)
		{
			var pos = (int)reader.BaseStream.Position;
			var off = reader.ReadInt32();
			return off != 0 ? pos + off : 0;
		}

		public static string ReadCString(this BinaryReader reader)
		{
			var list = new List<byte>(0x100);
			while (true)
			{
				var b = reader.ReadByte();
				if (b == 0)
					break;

				list.Add(b);
			}

			return Encoding.UTF8.GetString(list.ToArray());
		}
	}
}
