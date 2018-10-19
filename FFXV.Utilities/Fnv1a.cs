namespace FFXV.Utilities
{
	public class Fnv1a : IHashing<uint>
	{
		uint hval = 0;

		public uint GetDigest() => hval;

		public void Init()
		{
			hval = 0x811c9dc5U;
		}

		public void Write(byte[] data, uint offset, uint size)
		{
			for (int i = 0; i < size; i++)
			{
				WriteByte(data[offset + i]);
			}
		}

		public void WriteByte(byte b)
		{
			hval ^= b;
			hval *= 0x01000193;
		}
	}
}
