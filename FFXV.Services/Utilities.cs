using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXV.Services
{
	public static class Utilities
	{
		private static readonly List<byte> _buffer = new List<byte>(0x100);

		public static string ReadCString(this Stream stream) =>
			ReadCString(stream, Encoding.UTF8);

		public static string ReadCString(this Stream stream, Encoding encoding)
		{
			_buffer.Clear();

			byte c;
			while ((c = (byte)stream.ReadByte()) != 0)
			{
				_buffer.Add(c);
			}

			return encoding.GetString(_buffer.ToArray());
		}

		public static int WriteCString(this Stream stream, string str) =>
			WriteCString(stream, str, Encoding.UTF8);

		public static int WriteCString(this Stream stream, string str, Encoding encoding)
		{
			var buffer = encoding.GetBytes(str);
			stream.Write(buffer, 0, buffer.Length);
			stream.WriteByte(0);

			return buffer.Length + 1;
		}
	}
}
