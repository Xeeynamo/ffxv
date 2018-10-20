using FFXV.Utilities;
using SQEX.Ebony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace FFXV.Services
{
	public partial class Xmb2
	{
		private const bool AttributeDictionaryTest = true;

		private enum ValueType
		{
			ElementValue = 0,
			String = 1,
			Bool = 2,
			Signed = 3,
			Unsigned = 4,
			Float = 5,
			Double = 6,
			Float2 = 7,
			Float3 = 8,
			Float4 = 9
		}

		private class Element
		{
			public int ElementTableOffset { get; set; }

			public int VariantTableOffset { get; set; }

			public int ElementCount { get; set; }

			public int Unknown { get; set; }

			public int AttributeCount { get; set; }

			public List<Element> Elements { get; set; }

			public List<Attribute> Attributes { get; set; }

			public Variant Name { get; set; }

			public Variant Value { get; set; }

			public override string ToString() => Name?.ToString() ?? base.ToString();

			public static Element Read(BinaryReader reader, int offset)
			{
				reader.BaseStream.Position = offset;
				return new Element()
				{
					ElementTableOffset = reader.ReadOffset(),
					VariantTableOffset = reader.ReadOffset(),
					ElementCount = reader.ReadByte(),
					Unknown = reader.ReadInt16(),
					AttributeCount = reader.ReadByte(),
					Elements = new List<Element>(),
					Attributes = new List<Attribute>()
				};
			}
		}

		private class Attribute
		{
			public Variant Variant { get; }

			public string Name { get; }

			public string Value => Variant.Name;

			public Attribute(Variant variant)
			{
				Variant = variant;
				if (AttributeDictionaryTest)
				{
					Name = Names[variant.Hash];
				}
				else
				{
					Name = Names.TryGetValue(variant.Hash, out var str) ? str : variant.Hash.ToString("X08");
				}
			}

			public override string ToString() => $"{Name}={Value}";
		}

		private class Variant
		{
			public ValueType Type { get; internal set; }

			public uint Hash { get; private set; }

			public object Value { get; private set; }

			public string Name => Value?.ToString();

			public override string ToString() => Name;

			public static Variant Read(BinaryReader reader, int offset)
			{
				reader.BaseStream.Position = offset;
				var variant = new Variant()
				{
					Type = (ValueType)reader.ReadByte(),
					Hash = reader.ReadUInt32(),
				};
				
				switch (variant.Type)
				{
					case ValueType.String:
						reader.BaseStream.Position = reader.ReadOffset();
						variant.Value = reader.ReadCString();
						break;
					case ValueType.Bool:
						variant.Value = reader.ReadByte() != 0 ? true : false;
						break;
					case ValueType.Signed:
						variant.Value = reader.ReadInt32();
						break;
					case ValueType.Unsigned:
						variant.Value = reader.ReadUInt32();
						break;
					case ValueType.Float:
						variant.Value = reader.ReadSingle();
						break;
					case ValueType.Double:
						reader.BaseStream.Position = reader.ReadOffset();
						variant.Value = reader.ReadDouble();
						break;
					case ValueType.Float2:
						reader.BaseStream.Position = reader.ReadOffset();
						variant.Value = new Float2()
						{
							x = reader.ReadSingle(),
							y = reader.ReadSingle(),
						};
						break;
					case ValueType.Float3:
						reader.BaseStream.Position = reader.ReadOffset();
						variant.Value = new Float3()
						{
							x = reader.ReadSingle(),
							y = reader.ReadSingle(),
							z = reader.ReadSingle(),
						};
						break;
					case ValueType.Float4:
						reader.BaseStream.Position = reader.ReadOffset();
						variant.Value = new Float4()
						{
							x = reader.ReadSingle(),
							y = reader.ReadSingle(),
							z = reader.ReadSingle(),
							w = reader.ReadSingle(),
						};
						break;
					default:
						throw new ArgumentException($"Type {variant.Type} not recognized");
				}

				return variant;
			}
		}

		private static readonly Fnv1a hash = new Fnv1a();
		private static readonly Dictionary<uint, string> Names = new[]
		{
			"",
			"name",
			"objectIndex",
			"type",
			"path",
			"checked",
			"reference",
			"object",
			"original_type",
			"owner",
			"ownerIndex",
			"ownerPath",
		}.ToDictionary(x => hash.GetDigest(x), x => x);

		private Element rootElement;

		public XDocument Document => new XDocument(GetElement(rootElement));

		public Xmb2(BinaryReader reader)
		{
			var identifier = reader.ReadUInt32();
			var fileSize = reader.ReadInt32();
			var flags = reader.ReadInt16();
			var version = reader.ReadInt16();

			rootElement = ReadElement(reader, reader.ReadOffset());
		}

		private Element ReadElement(BinaryReader reader, int offset)
		{
			var element = Element.Read(reader, offset);

			var variantOffset = reader.ReadOffset(element.VariantTableOffset);
			element.Name = Variant.Read(reader, variantOffset);

			if (element.ElementCount > 0)
			{
				for (int i = 0; i < element.ElementCount; i++)
				{
					var child = ReadElement(reader, reader.ReadOffset(element.ElementTableOffset + i * 4));
					element.Elements.Add(child);
				}
			}
			else if (element.ElementTableOffset != 0)
			{
				element.Value = Variant.Read(reader, element.ElementTableOffset);
			}

			for (int i = 0; i < element.AttributeCount; i++)
			{
				var attributeOffset = reader.ReadOffset(element.VariantTableOffset + i * 4 + 4);
				element.Attributes.Add(new Attribute(Variant.Read(reader, attributeOffset)));
			}

			return element;
		}

		private XElement GetElement(Element element)
		{
			var xmlElement = new XElement(element.Name.Name);
			if (element.Value != null)
			{
				xmlElement.Value = element.Value.Name;
			}
			xmlElement.Add(element.Attributes.Select(x => new XAttribute(x.Name, x.Value)));
			xmlElement.Add(element.Elements.Select(x => GetElement(x)));
			return xmlElement;
		}
	}
}
