using System.Text;

namespace FFXV.Utilities
{
	public static class HashingExtension
	{
		public static T GetDigest<T>(this IHashing<T> hashing, string text)
		{
			return hashing.GetDigest(text, Encoding.UTF8);
		}

		public static T GetDigest<T>(this IHashing<T> hashing, string text, Encoding encoding)
		{
			hashing.Init();
			var data = encoding.GetBytes(text);
			hashing.Write(data, 0, (uint)data.Length);
			return hashing.GetDigest();
		}

		public static T GetDigest<T>(this IHashing<T> hashing, params int[] values)
		{
			hashing.Init();
			hashing.Write(values);
			return hashing.GetDigest();
		}

		public static void Write<T>(this IHashing<T> hashing, params int[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				var v = values[i];
				hashing.WriteByte((byte)(v >> 0));
				hashing.WriteByte((byte)(v >> 8));
				hashing.WriteByte((byte)(v >> 16));
				hashing.WriteByte((byte)(v >> 24));
			}
		}
	}
}
