namespace FFXV.Utilities
{
	public interface IHashing<T>
	{
		void Init();
		void WriteByte(byte b);
		void Write(byte[] data, uint offset, uint size);

		T GetDigest();
	}
}
