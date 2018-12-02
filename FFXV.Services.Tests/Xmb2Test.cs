using System.IO;
using System.Xml.Linq;
using Xunit;

namespace FFXV.Services.Tests
{
	public class Xmb2Test
	{
		[Fact]
		public void DeserializationTest()
		{
			XDocument doc;
			using (var stream = File.OpenRead("xmb2/cam_quest.exml"))
			{
				doc = new Xmb2(new BinaryReader(stream)).Document;
			}
		}

		[Fact]
		public void SerializationTest()
		{
			MemoryStream streamExpected, streamActual;
			using (var stream = File.OpenRead("xmb2/cam_quest.exml"))
			{
				streamExpected = new MemoryStream((int)stream.Length);
				streamActual = new MemoryStream((int)stream.Length);
				stream.CopyTo(streamExpected);
				streamExpected.Position = 0;
			}

			Xmb2.Create(new Xmb(new Xmb2(new BinaryReader(streamExpected)).Document), new BinaryWriter(streamActual));

			streamExpected.Position = 0;
			streamActual.Position = 0;

			Assert.Equal(streamExpected.Length, streamActual.Length);
			while (streamExpected.Position < streamActual.Position)
			{
				Assert.Equal(streamExpected.ReadByte(), streamActual.ReadByte());
			}
		}

		[Fact]
		public void WriterTest()
		{
			//var output = new MemoryStream();
			//using (var stream = File.OpenRead("xmb2/cam_quest.exml"))
			//{
			//	new Xmb2(new BinaryReader(stream)).Write(new BinaryWriter(output));
			//}

			var output = new MemoryStream();
			using (var stream = File.OpenRead("xmb2/cam_quest.exml"))
			{
				Xmb2.Create(new Xmb(new Xmb2(new BinaryReader(stream)).Document), new BinaryWriter(output));
			}

			output.Position = 0;
			var reader = new BinaryReader(output);
			Assert.Equal(0x32424D58, reader.ReadInt32());
			Assert.Equal(0x9C0, reader.ReadInt32());
			Assert.Equal(0, reader.ReadInt32());
			Assert.Equal(0x178, reader.ReadInt32());

			Assert.Equal(0, reader.ReadInt32());
			Assert.Equal(0x4B8, reader.ReadInt32());
			Assert.Equal(0, reader.ReadInt16());
			Assert.Equal(0, reader.ReadByte());
			Assert.Equal(3, reader.ReadByte());

			Assert.Equal(0, reader.ReadInt32());
			Assert.NotEqual(0x4BC, reader.ReadInt32());
			Assert.Equal(0, reader.ReadInt16());
			Assert.Equal(0, reader.ReadByte());
			Assert.Equal(3, reader.ReadByte());

			reader.BaseStream.Position = 0x184;
			Assert.NotEqual(0x344, reader.ReadInt32());
			Assert.NotEqual(0x49C, reader.ReadInt32());
			Assert.Equal(1, reader.ReadByte());
			Assert.Equal(0, reader.ReadByte());
			Assert.Equal(0, reader.ReadByte());
			Assert.Equal(1, reader.ReadByte());

			reader.BaseStream.Position = 0x178;
			Assert.NotEqual(0x33C, reader.ReadInt32());
			Assert.NotEqual(0x4A4, reader.ReadInt32());
			Assert.Equal(5, reader.ReadByte());
			Assert.Equal(0, reader.ReadByte());
			Assert.Equal(0, reader.ReadByte());
			Assert.Equal(0, reader.ReadByte());
		}
	}
}
