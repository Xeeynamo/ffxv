using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FFXV.Services
{
	public partial class Xmb
	{
		private static readonly bool OptimizeSize = false;

		private class XmbSave
		{
			private class StringEntry
			{
				public int Index { get; set; }

				public int Offset { get; set; }
			}

			private Header header;
			private List<Element> elements = new List<Element>();
			private List<Attribute> attributes = new List<Attribute>();
			private List<Variant> variants = new List<Variant>();
			private List<int> elementIndexes = new List<int>();
			private List<int> attributeIndexes = new List<int>();
			private Dictionary<string, StringEntry> strings = new Dictionary<string, StringEntry>();
			private int rootElementIndex;

			public void ReadRootElement(XElement element)
			{
				rootElementIndex = ReadElement(element);
			}

			public int ReadElement(XElement xmlElement)
			{
				var element = new Element()
				{
					NameStringOffset = GetOrAddString(xmlElement.Name.ToString()),
					Name = xmlElement.Name.ToString(),
				};

				var elementStrType = string.Empty;
				if (xmlElement.HasAttributes)
				{
					element.AttributeTableIndex = attributeIndexes.Count;

					foreach (var xmlAttribute in xmlElement.Attributes())
					{
						var valueType = GuessTypeFromAttributeName(xmlAttribute.Name.ToString());
						if (valueType == ValueType.Unknown)
						{
							var guessedValueType = GuessTypeFromValue(xmlAttribute.Value);
						}

						var attribute = new Attribute()
						{
							NameStringOffset = GetOrAddString(xmlAttribute.Name.ToString()),
							Name = xmlAttribute.Name.ToString(),
							VariantOffset = GetOrAddVariantIndex(valueType, xmlAttribute.Value)
						};

						if (!xmlElement.HasElements && xmlAttribute.Name == "type")
						{
							elementStrType = xmlAttribute.Value;
						}

						attributeIndexes.Add(AddComparable(attributes, attribute));
						element.AttributeCount++;
					}
				}

				if (xmlElement.HasElements)
				{
					var curElementIndexes = xmlElement.Elements()
						.Select(x => ReadElement(x))
						.ToArray();

					element.ElementTableIndex = elementIndexes.Count;
					element.ElementCount = curElementIndexes.Length;
					elementIndexes.AddRange(curElementIndexes);

					element.VariantOffset = GetOrAddVariantIndex(ValueType.Unknown, string.Empty);
				}
				else
				{
					var type = GetTypeFromAttributeName(elementStrType, xmlElement.Value);
					element.VariantOffset = GetOrAddVariantIndex(type, xmlElement.Value);
				}

				if (OptimizeSize)
				{
					return AddComparable(elements, element);
				}

				elements.Add(element);
				return elements.Count - 1;
			}

			public void Write(BinaryWriter writer)
			{
				const int TotalHeaderSize = 0x4C;

				var streamStringsTable = WriteChunk(strings, (w, x) =>
				{
					x.Value.Offset = (int)w.BaseStream.Position;
					w.BaseStream.WriteCString(x.Key);
				});

				foreach (var element in elements)
					element.NameStringOffset = strings[element.Name].Offset;

				foreach (var attribute in attributes)
					attribute.NameStringOffset = strings[attribute.Name].Offset;

				foreach (var variant in variants)
				{
					variant.NameStringOffset = strings[variant.Name].Offset;
					ProcessVariant(variant);
				}

				var streamElements = WriteChunk(elements, (w, x) => x.Write(w));
				var streamAttributes = WriteChunk(attributes, (w, x) => x.Write(w));
				var streamVariants = WriteChunk(variants, (w, x) => x.Write(w));
				var streamElementsTable = WriteChunk(elementIndexes, (w, x) => w.Write(x));
				var streamAttributesTable = WriteChunk(attributeIndexes, (w, x) => w.Write(x));

				header = new Header()
				{
					MagicCode = 0x00424D58,
					Elements = new HeaderEntry { Count = elements.Count },
					Attributes = new HeaderEntry { Count = attributes.Count },
					StringTable = new HeaderEntry { Count = (int)streamStringsTable.Length },
					ElementIndexTable = new HeaderEntry { Count = elementIndexes.Count },
					AttributeIndexTable = new HeaderEntry { Count = attributeIndexes.Count },
					Variants = new HeaderEntry { Count = variants.Count },
					RootElementIndex = rootElementIndex
				};

				int offset = TotalHeaderSize;
				header.Elements.Offset = offset;

				offset += (int)streamElements.Length;
				header.Attributes.Offset = offset;

				offset += (int)streamAttributes.Length;
				header.Variants.Offset = offset;

				offset += (int)streamVariants.Length;
				header.StringTable.Offset = offset;

				offset += (int)streamStringsTable.Length;
				header.ElementIndexTable.Offset = offset;

				offset += (int)streamElementsTable.Length;
				header.AttributeIndexTable.Offset = offset;

				header.Write(writer);
				writer.Write(0); // reserved data
				writer.Write(0); // reserved data
				writer.Write(0); // reserved data
				writer.Write(0); // reserved data
				System.Diagnostics.Debug.Assert(writer.BaseStream.Position == TotalHeaderSize);

				streamElements.CopyTo(writer.BaseStream);
				streamAttributes.CopyTo(writer.BaseStream);
				streamVariants.CopyTo(writer.BaseStream);
				streamStringsTable.CopyTo(writer.BaseStream);
				streamElementsTable.CopyTo(writer.BaseStream);
				streamAttributesTable.CopyTo(writer.BaseStream);
			}

			private Stream WriteChunk<T>(IEnumerable<T> collection, Action<BinaryWriter, T> writeFunc)
			{
				var writer = new BinaryWriter(new MemoryStream());
				foreach (var item in collection)
				{
					writeFunc(writer, item);
				}

				writer.BaseStream.Position = 0;
				return writer.BaseStream;
			}

			public void Compare(Xmb xmb)
			{
				System.Diagnostics.Debug.Assert(header.MagicCode == xmb.header.MagicCode);
				System.Diagnostics.Debug.Assert(header.RESERVED == xmb.header.RESERVED);
				System.Diagnostics.Debug.Assert(header.Elements.Offset == xmb.header.Elements.Offset);
				System.Diagnostics.Debug.Assert(header.Elements.Count == xmb.header.Elements.Count);
				System.Diagnostics.Debug.Assert(header.Attributes.Offset == xmb.header.Attributes.Offset);
				System.Diagnostics.Debug.Assert(header.Attributes.Count == xmb.header.Attributes.Count);
				System.Diagnostics.Debug.Assert(header.StringTable.Offset == xmb.header.StringTable.Offset);
				System.Diagnostics.Debug.Assert(header.StringTable.Count == xmb.header.StringTable.Count);
				System.Diagnostics.Debug.Assert(header.ElementIndexTable.Offset == xmb.header.ElementIndexTable.Offset);
				System.Diagnostics.Debug.Assert(header.ElementIndexTable.Count == xmb.header.ElementIndexTable.Count);
				System.Diagnostics.Debug.Assert(header.AttributeIndexTable.Offset == xmb.header.AttributeIndexTable.Offset);
				System.Diagnostics.Debug.Assert(header.AttributeIndexTable.Count == xmb.header.AttributeIndexTable.Count);
				System.Diagnostics.Debug.Assert(header.Variants.Offset == xmb.header.Variants.Offset);
				System.Diagnostics.Debug.Assert(header.Variants.Count == xmb.header.Variants.Count);
				System.Diagnostics.Debug.Assert(header.RootElementIndex == xmb.header.RootElementIndex);

				System.Diagnostics.Debug.Assert(elements.Count == xmb.elements.Length);
				System.Diagnostics.Debug.Assert(attributes.Count == xmb.attributes.Length);
				System.Diagnostics.Debug.Assert(variants.Count == xmb.variants.Length);
				System.Diagnostics.Debug.Assert(elementIndexes.Count == xmb.elementIndexTable.Length);
				System.Diagnostics.Debug.Assert(attributeIndexes.Count == xmb.attributeIndexTable.Length);

				for (int i = 0; i < elements.Count; i++)
				{
					System.Diagnostics.Debug.Assert(elements[i].AttributeTableIndex == xmb.elements[i].AttributeTableIndex);
					System.Diagnostics.Debug.Assert(elements[i].AttributeCount == xmb.elements[i].AttributeCount);
					System.Diagnostics.Debug.Assert(elements[i].ElementTableIndex == xmb.elements[i].ElementTableIndex);
					System.Diagnostics.Debug.Assert(elements[i].ElementCount == xmb.elements[i].ElementCount);
					System.Diagnostics.Debug.Assert(elements[i].VariantOffset == xmb.elements[i].VariantOffset);
					System.Diagnostics.Debug.Assert(elements[i].NameStringOffset == xmb.elements[i].NameStringOffset);
					System.Diagnostics.Debug.Assert(elements[i].Name == xmb.elements[i].Name);
				}

				for (int i = 0; i < attributes.Count; i++)
				{
					System.Diagnostics.Debug.Assert(attributes[i].VariantOffset == xmb.attributes[i].VariantOffset);
					System.Diagnostics.Debug.Assert(attributes[i].NameStringOffset == xmb.attributes[i].NameStringOffset);
					System.Diagnostics.Debug.Assert(attributes[i].Name == xmb.attributes[i].Name);
				}

				for (int i = 0; i < variants.Count; i++)
				{
					System.Diagnostics.Debug.Assert(variants[i].Type == xmb.variants[i].Type);
					System.Diagnostics.Debug.Assert(variants[i].NameStringOffset == xmb.variants[i].NameStringOffset);
					System.Diagnostics.Debug.Assert(variants[i].Value1 == xmb.variants[i].Value1);
					System.Diagnostics.Debug.Assert(variants[i].Value2 == xmb.variants[i].Value2);
					System.Diagnostics.Debug.Assert(variants[i].Value3 == xmb.variants[i].Value3);
					System.Diagnostics.Debug.Assert(variants[i].Value4 == xmb.variants[i].Value4);
					System.Diagnostics.Debug.Assert(variants[i].Name == xmb.variants[i].Name);
				}

				for (int i = 0; i < elementIndexes.Count; i++)
				{
					System.Diagnostics.Debug.Assert(elementIndexes[i] == xmb.elementIndexTable[i]);
				}

				for (int i = 0; i < attributeIndexes.Count; i++)
				{
					System.Diagnostics.Debug.Assert(attributeIndexes[i] == xmb.attributeIndexTable[i]);
				}
			}

			private static int AddComparable<T>(List<T> collection, T item)
				where T : IComparable<T>
			{
				for (int i = 0; i < collection.Count; i++)
				{
					if (collection[i].CompareTo(item) == 0)
						return i;
				}

				collection.Add(item);
				return collection.Count - 1;
			}

			private int GetOrAddString(string str)
			{
				int value;
				if (!strings.TryGetValue(str, out var entry))
				{
					strings[str] = new StringEntry()
					{
						Index = value = strings.Count
					};
				}
				else
				{
					value = entry.Index;
				}

				return value;
			}

			private int GetOrAddVariantIndex(ValueType type, string value)
			{
				var index = GetVariantIndex(type, value);
				if (index < 0)
				{
					index = variants.Count;

					variants.Add(new Variant()
					{
						Type = type,
						NameStringOffset = GetOrAddString(value),
						Name = value,
					});
				}

				return index;
			}

			private ValueType GuessTypeFromAttributeName(string name)
			{
				switch (name)
				{
					case "reference":
					case "checked":
						return ValueType.Bool;
					case "objectIndex":
					case "ownerIndex":
					case "value":
					case "fixid":
						return ValueType.Unsigned;
					default: return ValueType.Unknown;
				}
			}

			private ValueType GuessTypeFromValue(string value)
			{
				switch (value)
				{
					case "true":
					case "false":
					case "True":
					case "False":
						return ValueType.Bool;
					default:
						if (value.Length > 0)
						{
							if (value[0] == '-')
							{
								if (int.TryParse(value, out var v))
									return ValueType.Signed;
							}
							else
							{
								if (uint.TryParse(value, out var v))
									return ValueType.Unsigned;
							}
						}
						return ValueType.Unknown;
				}
			}

			private ValueType GetTypeFromAttributeName(string name, string value)
			{
				switch (name)
				{
					case "bool": return ValueType.Bool;
					case "double":
						{
							if (value.IndexOf('.') >= 0)
								return ValueType.Float;
							return int.Parse(value) >= 0 ?
								ValueType.Signed : ValueType.Unsigned;
						}
					case "float": return ValueType.Float;
					case "float2": return ValueType.Float2;
					case "float3": return ValueType.Float3;
					case "float4": return ValueType.Float4;
					default: return ValueType.Unknown;
				}
			}

			private int GetVariantIndex(ValueType type, string name)
			{
				for (int i = 0; i < variants.Count; i++)
				{
					if (variants[i].Type == type &&
						variants[i].Name == name)
					{
						return i;
					}
				}

				return -1;
			}

			private void ProcessVariant(Variant variant)
			{
				switch (variant.Type)
				{
					case ValueType.Bool:
						ProcessBoolean(variant);
						break;
					case ValueType.Signed:
						ProcessInteger(variant);
						break;
					case ValueType.Unsigned:
						ProcessInteger(variant);
						break;
					case ValueType.Float:
					case ValueType.Float2:
					case ValueType.Float3:
					case ValueType.Float4:
						ProcessFloat(variant);
						break;
				}
			}

			private static void ProcessBoolean(Variant variant)
			{
				if (!bool.TryParse(variant.Name.ToLower(), out var b))
				{
					throw new ArgumentException($"Unable to convert {variant.Name} for type {variant.Type}");
				}

				variant.Value1 = b ? 1 : 0;
			}

			private static void ProcessInteger(Variant variant)
			{
				if (!int.TryParse(variant.Name, out var v))
				{
					throw new ArgumentException($"Unable to convert {variant.Name} for type {variant.Type}");
				}

				variant.Value1 = v;
			}

			private static void ProcessFloat(Variant variant)
			{
				var values = variant.Name.Split(',');
				var fArray = new float[values.Length];
				for (int i = 0; i < fArray.Length; i++)
				{
					if (!float.TryParse(values[i], out var v))
					{
						throw new ArgumentException($"Unable to convert {variant.Name} for type {variant.Type}");
					}

					fArray[i] = v;
				}

				FromFloatArray(variant, fArray);
			}

			private static void FromFloatArray(Variant variant, float[] f)
			{
				if (f.Length > 0)
					variant.Value1 = FromFloat(f[0]);
				if (f.Length > 1)
					variant.Value1 = FromFloat(f[1]);
				if (f.Length > 2)
					variant.Value1 = FromFloat(f[2]);
				if (f.Length > 3)
					variant.Value1 = FromFloat(f[3]);
			}

			private static int FromFloat(float f)
			{
				var data = BitConverter.GetBytes(f);
				int v = (data[0] & 0xFF) << 0;
				v |= (data[1] & 0xFF) << 8;
				v |= (data[2] & 0xFF) << 16;
				v |= (data[3] & 0xFF) << 24;

				return v;
			}
		}

		public static void Save(Stream stream, XElement element)
		{
			var save = new XmbSave();
			save.ReadRootElement(element);
			save.Write(new BinaryWriter(stream));
		}

		public static void Compare(XElement element, Stream stream)
		{
			var save = new XmbSave();
			save.ReadRootElement(element);
			save.Write(new BinaryWriter(new MemoryStream()));
			save.Compare(new Xmb(new BinaryReader(stream)));
		}
	}
}
