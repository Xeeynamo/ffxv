using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXV.Services
{
	public partial class Xmb
	{
		public enum ValueType
		{
			Unknown,
			Bool,
			Signed,
			Unsigned,
			Float,
			Float2Alt,
			Float2,
			Float3,
			Float4,
		}

		private class HeaderEntry
		{
			public int Offset { get; set; }
			public int Count { get; set; }

			public static HeaderEntry Read(BinaryReader reader)
			{
				return new HeaderEntry()
				{
					Offset = reader.ReadInt32(),
					Count = reader.ReadInt32(),
				};
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write(Offset);
				writer.Write(Count);
			}
		}

		private class Header
		{
			public uint MagicCode { get; set; }
			public uint RESERVED { get; set; }
			public HeaderEntry Elements { get; set; }
			public HeaderEntry Attributes { get; set; }
			public HeaderEntry StringTable { get; set; }
			public HeaderEntry ElementIndexTable { get; set; }
			public HeaderEntry AttributeIndexTable { get; set; }
			public HeaderEntry Variants { get; set; }
			public int RootElementIndex { get; set; }

			public static Header Read(BinaryReader reader)
			{
				var magicCode = reader.ReadUInt32();
				if (magicCode != 0x00424D58)
					throw new ArgumentException("Not a XMB file");

				return new Header()
				{
					MagicCode = magicCode,
					RESERVED = reader.ReadUInt32(),
					Elements = HeaderEntry.Read(reader),
					Attributes = HeaderEntry.Read(reader),
					StringTable = HeaderEntry.Read(reader),
					ElementIndexTable = HeaderEntry.Read(reader),
					AttributeIndexTable = HeaderEntry.Read(reader),
					Variants = HeaderEntry.Read(reader),
					RootElementIndex = reader.ReadInt32(),
				};
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write(MagicCode);
				writer.Write(RESERVED);
				Elements.Write(writer);
				Attributes.Write(writer);
				StringTable.Write(writer);
				ElementIndexTable.Write(writer);
				AttributeIndexTable.Write(writer);
				Variants.Write(writer);
				writer.Write(RootElementIndex);
			}
		}

		public class Element : IComparable<Element>
		{
			public long Reserved { get; set; }
			public int AttributeTableIndex { get; set; }
			public int AttributeCount { get; set; }
			public int ElementTableIndex { get; set; }
			public int ElementCount { get; set; }
			public int NameStringOffset { get; set; }
			public int VariantOffset { get; set; }

			public string Name { get; set; }

			public override string ToString() => Name;

			public static Element Read(BinaryReader reader)
			{
				return new Element()
				{
					Reserved = reader.ReadInt64(),
					AttributeTableIndex = reader.ReadInt32(),
					AttributeCount = reader.ReadInt32(),
					ElementTableIndex = reader.ReadInt32(),
					ElementCount = reader.ReadInt32(),
					NameStringOffset = reader.ReadInt32(),
					VariantOffset = reader.ReadInt32(),
				};
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write((long)0);
				writer.Write(AttributeTableIndex);
				writer.Write(AttributeCount);
				writer.Write(ElementTableIndex);
				writer.Write(ElementCount);
				writer.Write(NameStringOffset);
				writer.Write(VariantOffset);
			}

			public int CompareTo(Element other)
			{
				return Reserved == other.Reserved &&
					AttributeTableIndex == other.AttributeTableIndex &&
					AttributeCount == other.AttributeCount &&
					ElementTableIndex == other.ElementTableIndex &&
					ElementCount == other.ElementCount &&
					NameStringOffset == other.NameStringOffset &&
					VariantOffset == other.VariantOffset ?
					0 : 1;
			}
		}

		public class Attribute : IComparable<Attribute>
		{
			public long Reserved { get; set; }
			public int NameStringOffset { get; set; }
			public int VariantOffset { get; set; }

			public string Name { get; set; }

			public override string ToString() => Name;

			public static Attribute Read(BinaryReader reader)
			{
				return new Attribute()
				{
					Reserved = reader.ReadInt64(),
					NameStringOffset = reader.ReadInt32(),
					VariantOffset = reader.ReadInt32(),
				};
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write((long)0);
				writer.Write(NameStringOffset);
				writer.Write(VariantOffset);
			}

			public int CompareTo(Attribute other)
			{
				return Reserved == other.Reserved &&
					NameStringOffset == other.NameStringOffset &&
					VariantOffset == other.VariantOffset ?
					0 : 1;
			}
		}

		public class Variant : IComparable<Variant>
		{
			public ValueType Type { get; set; }
			public int NameStringOffset { get; set; }
			public int Value1 { get; set; }
			public int Value2 { get; set; }
			public int Value3 { get; set; }
			public int Value4 { get; set; }

			public string Name { get; set; }

			public override string ToString() => Name;

			public static Variant Read(BinaryReader reader)
			{
				return new Variant()
				{
					Type = (ValueType)reader.ReadInt32(),
					NameStringOffset = reader.ReadInt32(),
					Value1 = reader.ReadInt32(),
					Value2 = reader.ReadInt32(),
					Value3 = reader.ReadInt32(),
					Value4 = reader.ReadInt32(),
				};
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write((int)Type);
				writer.Write(NameStringOffset);
				writer.Write(Value1);
				writer.Write(Value2);
				writer.Write(Value3);
				writer.Write(Value4);
			}

			public int CompareTo(Variant other)
			{
				return Type == other.Type &&
					NameStringOffset == other.NameStringOffset &&
					Value1 == other.Value1 &&
					Value2 == other.Value2 &&
					Value3 == other.Value3 &&
					Value4 == other.Value4 &&
					Name == other.Name ?
					0 : 1;
			}
		}

		private const bool EnableIeeeHack = true;
		private Header header;
		private Element[] elements;
		private Attribute[] attributes;
		private Variant[] variants;
		private int[] elementIndexTable;
		private int[] attributeIndexTable;
		private int rootElement;

		private Xmb(BinaryReader reader)
		{
			header = Header.Read(reader);
			rootElement = header.RootElementIndex < header.Elements.Count ?
				header.RootElementIndex : 0;

			reader.BaseStream.Position = header.Elements.Offset;
			elements = new Element[header.Elements.Count];
			for (int i = 0; i < elements.Length; i++)
			{
				elements[i] = Element.Read(reader);
			}

			reader.BaseStream.Position = header.Attributes.Offset;
			attributes = new Attribute[header.Attributes.Count];
			for (int i = 0; i < attributes.Length; i++)
			{
				attributes[i] = Attribute.Read(reader);
			}

			reader.BaseStream.Position = header.Variants.Offset;
			variants = new Variant[header.Variants.Count];
			for (int i = 0; i < variants.Length; i++)
			{
				variants[i] = Variant.Read(reader);
			}

			reader.BaseStream.Position = header.ElementIndexTable.Offset;
			elementIndexTable = new int[header.ElementIndexTable.Count];
			for (int i = 0; i < elementIndexTable.Length; i++)
			{
				elementIndexTable[i] = reader.ReadInt32();
			}

			reader.BaseStream.Position = header.AttributeIndexTable.Offset;
			attributeIndexTable = new int[header.AttributeIndexTable.Count];
			for (int i = 0; i < attributeIndexTable.Length; i++)
			{
				attributeIndexTable[i] = reader.ReadInt32();
			}

			for (int i = 0; i < elements.Length; i++)
			{
				elements[i].Name = ReadString(reader.BaseStream, header.StringTable.Offset + elements[i].NameStringOffset);
			}

			for (int i = 0; i < attributes.Length; i++)
			{
				attributes[i].Name = ReadString(reader.BaseStream, header.StringTable.Offset + attributes[i].NameStringOffset);
			}

			for (int i = 0; i < variants.Length; i++)
			{
				variants[i].Name = ReadString(reader.BaseStream, header.StringTable.Offset + variants[i].NameStringOffset);
			}
		}

		private string ReadString(Stream stream, int offset)
		{
			stream.Position = offset;
			return stream.ReadCString();
		}

		private static float ToFloat(int n)
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(n), 0);
		}
	}
}
