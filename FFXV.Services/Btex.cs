//using System;
//using System.Drawing;
//using System.IO;
//using System.Text;
//using Xe.Imaging;

//namespace Xe.Reverse.Sqex.Ffxv
//{
//	public class Btex : Imaging.IBitmap
//	{
//		const ulong MagicCode1 = 0x7865746242444553UL; // SEDBbtex
//		const uint MagicCode2 = 0x58455442U; // BTEX

//		private Imaging.PS4.Gnf _gnf;

//		public string Name { get; set; }

//		public Size Size => _gnf.Size;

//		public Btex(BinaryReader reader)
//		{
//			if (reader.ReadUInt64() != MagicCode1)
//				throw new InvalidDataException("Not a valid SEDBbtex stream.");
//			reader.ReadUInt64();
//			var fileSize = reader.ReadUInt64();
//			reader.BaseStream.Position += 0xE8;

//			if (reader.ReadUInt32() != MagicCode2)
//				throw new InvalidDataException("Not a valid BTEX stream.");
//			var version = reader.ReadInt32();
//			var gnfLength = reader.ReadInt32();
//			var unk0c = reader.ReadInt16();
//			var unk0e = reader.ReadInt16();
//			var unk10 = reader.ReadInt16();
//			var unk12 = reader.ReadInt16();
//			var unk14 = reader.ReadInt32();
//			var unk18 = reader.ReadInt32();
//			var unk1c = reader.ReadInt32();
//			var width = reader.ReadInt16();
//			var height = reader.ReadInt16();
//			var unk24 = reader.ReadInt16();
//			var unk26 = reader.ReadInt16();
//			var unk28 = reader.ReadInt16();
//			var unk2a = reader.ReadInt16();
//			var unk2c = reader.ReadInt16();
//			var unk2e = reader.ReadInt16();
//			var unk30 = reader.ReadInt32();
//			var unk34 = reader.ReadInt32();
//			var unk38 = reader.ReadInt32();
//			var gnfLength2 = reader.ReadInt32();
//			var unk40 = reader.ReadInt32();
//			var unk44 = reader.ReadInt32();
//			var unk48 = reader.ReadInt32();
//			var unk4c = reader.ReadInt32();
//			var unk50 = reader.ReadInt32();
//			var unk54 = reader.ReadInt32();

//			byte[] name = new byte[0xA8];
//			reader.Read(name, 0, name.Length);
//			Name = Encoding.UTF8.GetString(name).Trim();
//			_gnf = new Imaging.PS4.Gnf(reader);
//		}

//		public BitmapData ToRgba32()
//		{
//			return _gnf.ToRgba32();
//		}
//	}
//}
