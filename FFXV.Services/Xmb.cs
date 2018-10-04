using FFXV.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FFXV.Services
{
	public class Xmb
	{
		public enum ValueType
		{
			Unknown,
			Bool,
			Signed,
			Unsigned,
			Float,
			Vector2,
			Vector3,
			Vector4,
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
		}

		public class Element
		{
			public long Reserved { get; set; }
			public int AttributeTableIndex { get; set; }
			public int AttributeCount { get; set; }
			public int ElementTableIndex { get; set; }
			public int ElementCount { get; set; }
			public int NameStringOffset { get; set; }
			public int VariantOffset { get; set; }

			public string Name { get; set; }

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
		}

		public class Attribute
		{
			public long Reserved { get; set; }
			public int NameStringOffset { get; set; }
			public int VariantTableIndex { get; set; }

			public string Name { get; set; }

			public static Attribute Read(BinaryReader reader)
			{
				return new Attribute()
				{
					Reserved = reader.ReadInt64(),
					NameStringOffset = reader.ReadInt32(),
					VariantTableIndex = reader.ReadInt32(),
				};
			}
		}

		public class Variant
		{
			public ValueType Type { get; set; }
			public int StringOffset { get; set; }
			public int Value1 { get; set; }
			public int Value2 { get; set; }
			public int Value3 { get; set; }
			public int Value4 { get; set; }

			public string Name { get; set; }

			public override string ToString()
			{
				switch (Type)
				{
					case ValueType.Unknown:
						return Value1 == 0 && Value2 == 0 && Value3 == 0 && Value4 == 0 ? "" : "???";
					case ValueType.Bool:
						return (Value1 != 0).ToString();
					case ValueType.Signed:
						return Value1.ToString();
					case ValueType.Unsigned:
						return Value1.ToString();
					case ValueType.Vector2:
						return $"{ToFloat(Value1)},{ToFloat(Value2)}";
					case ValueType.Vector3:
						return $"{ToFloat(Value1)},{ToFloat(Value2)},{ToFloat(Value3)}";
					case ValueType.Vector4:
						return $"{ToFloat(Value1)},{ToFloat(Value2)},{ToFloat(Value3)},{ToFloat(Value4)}";
					default:
						return $"{Type}?? {Value1.ToString("X08")} {Value2.ToString("X08")} {Value3.ToString("X08")} {Value4.ToString("X08")}";
				}
			}

			public static Variant Read(BinaryReader reader)
			{
				return new Variant()
				{
					Type = (ValueType)reader.ReadInt32(),
					StringOffset = reader.ReadInt32(),
					Value1 = reader.ReadInt32(),
					Value2 = reader.ReadInt32(),
					Value3 = reader.ReadInt32(),
					Value4 = reader.ReadInt32(),
				};
			}
		}

		private int rootElement;
		private Element[] elements;
		private Attribute[] attributes;
		private Variant[] variants;
		private int[] elementIndexTable;
		private int[] attributeIndexTable;

		public Xmb(BinaryReader reader)
		{
			var header = Header.Read(reader);
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
				elements[i].Name = ReadString(reader, header.StringTable.Offset + elements[i].NameStringOffset);
			}

			for (int i = 0; i < attributes.Length; i++)
			{
				attributes[i].Name = ReadString(reader, header.StringTable.Offset + attributes[i].NameStringOffset);
			}

			for (int i = 0; i < variants.Length; i++)
			{
				variants[i].Name = ReadString(reader, header.StringTable.Offset + variants[i].StringOffset);
			}
		}

		private XElement ReadNode(Element xmbElement)
		{
			var element = new XElement(xmbElement.Name);

			var variantValue = variants[xmbElement.VariantOffset];
			element.Value = variantValue.Name;

			for (int i = 0; i < xmbElement.AttributeCount; i++)
			{
				var attributeIndex = attributeIndexTable[xmbElement.AttributeTableIndex + i];
				var attribute = attributes[attributeIndex];
				var value = variants[attribute.VariantTableIndex];
				element.Add(new XAttribute(attribute.Name, value.Name));
			}

			for (int i = 0; i < xmbElement.ElementCount; i++)
			{
				var elementIndex = elementIndexTable[xmbElement.ElementTableIndex + i];
				var node = ReadNode(elements[elementIndex]);
				element.Add(node);
			}

			return element;
		}

		public XElement GetXDocument()
		{
			return ReadNode(elements[rootElement]);
		}

		private string ReadString(BinaryReader reader, int offset)
		{
			reader.BaseStream.Position = offset;
			return ReadString(reader);
		}

		List<byte> _buffer = new List<byte>(0x100);
		private string ReadString(BinaryReader reader)
		{
			_buffer.Clear();

			byte c;
			bool nonCharacterFound = false;
			while ((c = reader.ReadByte()) != 0)
			{
				nonCharacterFound = c >= 0x00 && c <= 0x1f && c != 0x0a;
				_buffer.Add(c);
			}

			if (nonCharacterFound)
				nonCharacterFound = !nonCharacterFound;

			var data = _buffer.ToArray();
			return nonCharacterFound ?
				Convert.ToBase64String(data) :
				Encoding.UTF8.GetString(data);
		}

		private static float ToFloat(int n)
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(n), 0);
		}
	}
}
