using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace FFXV.Services
{
	public partial class Xmb
	{
		private const bool EnableIeeeHack = true;
		private static readonly bool OptimizeSize = true;

		private class StringEntry
		{
			public int Index { get; set; }

			public int Offset { get; set; }
		}

		private Dictionary<int, int> dicElements;
		private Dictionary<int, int> dicAttributes;
		private Dictionary<int, int> dicVariants;
		private Dictionary<string, StringEntry> strings;

		public Xmb(XDocument document) :
			this()
		{
			ReadRootElement(document.Root);
			EvaluateOffsetsAndData();
		}

		private void ReadRootElement(XElement element)
		{
			dicElements = new Dictionary<int, int>();
			dicAttributes = new Dictionary<int, int>();
			dicVariants = new Dictionary<int, int>();
			strings = new Dictionary<string, StringEntry>();
			rootElementIndex = ReadElement(element);
		}

		private int ReadElement(XElement xmlElement)
		{
			var element = new Element()
			{
				NameStringOffset = AddString(xmlElement.Name.ToString()),
				Name = xmlElement.Name.ToString(),
			};

			var elementStrType = string.Empty;
			if (xmlElement.HasAttributes)
			{
				element.AttributeTableIndex = _attributeIndexTable.Count;

				foreach (var xmlAttribute in xmlElement.Attributes())
				{
					var valueType = GuessTypeFromValue(xmlAttribute.Value);

					var attribute = new Attribute()
					{
						NameStringOffset = AddString(xmlAttribute.Name.ToString()),
						Name = xmlAttribute.Name.ToString(),
						VariantOffset = GetOrAddVariantIndex(valueType, xmlAttribute.Value)
					};

					if (!xmlElement.HasElements && xmlAttribute.Name == "type")
					{
						elementStrType = xmlAttribute.Value;
					}

					_attributeIndexTable.Add(AddComparable(dicAttributes, _attributes, attribute));
					element.AttributeCount++;
				}
			}

			if (xmlElement.HasElements)
			{
				var curElementIndexes = xmlElement.Elements()
					.Select(x => ReadElement(x))
					.ToArray();

				element.ElementTableIndex = _elementIndexTable.Count;
				element.ElementCount = curElementIndexes.Length;
				_elementIndexTable.AddRange(curElementIndexes);

				element.VariantOffset = GetOrAddVariantIndex(ValueType.Unknown, string.Empty);
			}
			else
			{
				var type = GetTypeFromAttribute(elementStrType, xmlElement.Value);
				element.VariantOffset = GetOrAddVariantIndex(type, xmlElement.Value);
			}

			if (OptimizeSize)
			{
				return AddComparable(dicElements, _elements, element);
			}

			_elements.Add(element);
			return _elements.Count - 1;
		}

		private void EvaluateOffsetsAndData()
		{
			StringData = WriteChunk(strings, (w, x) =>
			{
				x.Value.Offset = (int)w.BaseStream.Position;
				w.BaseStream.WriteCString(x.Key);
			});

			foreach (var element in _elements)
				element.NameStringOffset = strings[element.Name].Offset;

			foreach (var attribute in _attributes)
				attribute.NameStringOffset = strings[attribute.Name].Offset;

			foreach (var variant in _variants)
			{
				variant.NameStringOffset = strings[variant.Name].Offset;
				ProcessVariant(variant);
			}
		}

		public void Write(BinaryWriter writer)
		{
			const int TotalHeaderSize = 0x4C;

			var dataElements = WriteChunk(_elements, (w, x) => x.Write(w));
			var dataAttributes = WriteChunk(_attributes, (w, x) => x.Write(w));
			var dataVariants = WriteChunk(_variants, (w, x) => x.Write(w));
			var dataElementsTable = WriteChunk(_elementIndexTable, (w, x) => w.Write(x));
			var dataAttributesTable = WriteChunk(_attributeIndexTable, (w, x) => w.Write(x));

			var header = new Header()
			{
				MagicCode = 0x00424D58,
				Elements = new HeaderEntry { Count = _elements.Count },
				Attributes = new HeaderEntry { Count = _attributes.Count },
				StringTable = new HeaderEntry { Count = StringData.Length },
				ElementIndexTable = new HeaderEntry { Count = _elementIndexTable.Count },
				AttributeIndexTable = new HeaderEntry { Count = _attributeIndexTable.Count },
				Variants = new HeaderEntry { Count = _variants.Count },
				RootElementIndex = rootElementIndex
			};

			int offset = TotalHeaderSize;
			header.Elements.Offset = offset;

			offset += dataElements.Length;
			header.Attributes.Offset = offset;

			offset += dataAttributes.Length;
			header.Variants.Offset = offset;

			offset += dataVariants.Length;
			header.StringTable.Offset = offset;

			offset += StringData.Length;
			header.ElementIndexTable.Offset = offset;

			offset += dataElementsTable.Length;
			header.AttributeIndexTable.Offset = offset;

			header.Write(writer);
			writer.Write(0); // reserved data
			writer.Write(0); // reserved data
			writer.Write(0); // reserved data
			writer.Write(0); // reserved data
			System.Diagnostics.Debug.Assert(writer.BaseStream.Position == TotalHeaderSize);

			writer.Write(dataElements);
			writer.Write(dataAttributes);
			writer.Write(dataVariants);
			writer.Write(StringData);
			writer.Write(dataElementsTable);
			writer.Write(dataAttributesTable);
		}

		private byte[] WriteChunk<T>(IEnumerable<T> collection, Action<BinaryWriter, T> writeFunc)
		{
			var stream = new MemoryStream(0x10000);
			var writer = new BinaryWriter(stream);
			foreach (var item in collection)
			{
				writeFunc(writer, item);
			}

			stream.Position = 0;
			return new BinaryReader(stream).ReadBytes((int)stream.Length);
		}

		private int GetOrAddVariantIndex(ValueType type, string value)
		{
			return AddComparable(dicVariants, _variants, new Variant()
			{
				Type = type,
				NameStringOffset = AddString(value),
				Name = value,
			});
		}

		private int AddString(string str)
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

		private static int AddComparable<T>(Dictionary<int, int> dictionary, List<T> collection, T item)
			where T : IComparable<T>
		{
			AddComparable(dictionary, collection, item, out var result);
			return result;
		}

		private static bool AddComparable<T>(Dictionary<int, int> dictionary, List<T> collection, T item, out int index)
			where T : IComparable<T>
		{
			var hash = item.GetHashCode();
			if (!dictionary.TryGetValue(hash, out index))
			{
				dictionary[index] = index = collection.Count;
				collection.Add(item);
				return true;
			}

			return false;
		}

		private static ValueType GetTypeFromAttribute(string type, string value)
		{
			switch (type)
			{
				case "bool":
					return ValueType.Bool;
				default:
					return GuessTypeFromValue(value);
			}
		}

		public static ValueType GuessTypeFromValue(string value)
		{
			switch (value)
			{
				case null:
					return ValueType.Unknown;
				case "true":
				case "false":
				case "True":
				case "False":
					return ValueType.Bool;
				default:
					if (value.Length > 0)
					{
						if (value.Contains(",") || value.Contains("."))
						{
							// During an acceptance test on debug_wm.exml I found that
							// the value -8.651422E-06,-145,8.651422E-06,1 is recognized
							// as Float4 by my parser, but the actual file has Unknown as
							// a type, which is odd. My opinion is that the parser in SQEX
							// house is so stupid that recognize the f.ffE-XX as a string.
							// This is why there is this awkard hack...
							if (EnableIeeeHack && value.Contains("E"))
								return ValueType.Unknown;

							var values = value.Split(',');
							if (float.TryParse(values[0], out var v))
							{
								switch (values.Length)
								{
									case 1: return ValueType.Float;
									case 2: return ValueType.Float2;
									case 3: return ValueType.Float3;
									case 4: return ValueType.Float4;
								}
							}
						}
						else
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
					}
					return ValueType.Unknown;
			}
		}

		private static void ProcessVariant(Variant variant)
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
			FromFloatArray(variant, ConvertFloatsFromString(variant.Name));
		}

		private static float[] ConvertFloatsFromString(string value)
		{
			var values = value.Split(',');
			var fArray = new float[values.Length];
			for (int i = 0; i < fArray.Length; i++)
			{
				if (!float.TryParse(values[i], out var v))
				{
					throw new ArgumentException($"Unable to convert {value}");
				}

				fArray[i] = v;
			}

			return fArray;
		}

		private static void FromFloatArray(Variant variant, float[] f)
		{
			if (f.Length > 0)
				variant.Value1 = FromFloat(f[0]);
			if (f.Length > 1)
				variant.Value2 = FromFloat(f[1]);
			if (f.Length > 2)
				variant.Value3 = FromFloat(f[2]);
			if (f.Length > 3)
				variant.Value4 = FromFloat(f[3]);
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
}
