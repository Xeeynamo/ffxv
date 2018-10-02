using System;
using System.IO;
using System.Text;

namespace Xe
{
	public static class Extensions
	{
		/// <summary>
		/// Read a string with the given encoding, until a null terminator is reached.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="maxLength"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string ReadCString(this BinaryReader reader,
			int maxLength = 0x100,
			Encoding encoding = null)
		{
			if (encoding == null)
				encoding = Encoding.UTF8;

			int length;
			var data = new byte[maxLength];
			for (length = 0; length < maxLength;)
			{
				byte c = reader.ReadByte();
				if (c == 0)
					break;
				data[length++] = c;
			}
			return encoding.GetString(data, 0, length);
		}

		public static int WriteStringData(this BinaryWriter writer, string str)
		{
			return WriteStringData(writer, str, Encoding.ASCII);
		}
		public static int WriteStringData(this BinaryWriter writer, string str, Encoding encoding)
		{
			var bytes = encoding.GetBytes(str ?? string.Empty);
			byte len = (byte)Math.Min(bytes.Length, byte.MaxValue);
			writer.Write(len);
			writer.Write(bytes, 0, len);
			return len;
		}

		public static int ToInt(this Guid guid)
		{
			return guid.GetHashCode();
		}
		public static bool IntCollide(this Guid guid, Guid guid2)
		{
			return guid.GetHashCode() == guid2.GetHashCode();
		}

		/// <summary>
		/// Open a binary file from the given file name in read mode.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static BinaryReader ReadBinaryFile(this string fileName)
		{
			if (!File.Exists(fileName))
				return null;
			return new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}
	}
}
