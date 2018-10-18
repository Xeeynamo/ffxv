using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace FFXV.Services.Tests
{
	public class XmbTest
	{
		[Fact]
		public void XmbAcceptanceTest()
		{
			XDocument document;
			using (var stream = File.OpenRead("xmb/acceptance-test.xml"))
			{
				document = XDocument.Load(stream);
			}

			var xmb = new Xmb(document);
			At(xmb.Elements, xmb.RootElementIndex, e1 =>
			{
				Assert.Equal("not-a-package", e1.Name);
				Assert.Equal(1, e1.ElementCount);
				Assert.Equal(1, e1.AttributeCount);

				At(xmb.Variants, e1.VariantOffset, variant =>
				{
					Assert.Equal(string.Empty, variant.Name);
				});

				At(xmb.Attributes, xmb.AttributeIndexTable, e1.AttributeTableIndex, attribute =>
				{
					Assert.Equal("name", attribute.Name);
					At(xmb.Variants, attribute.VariantOffset, variant =>
					{
						Assert.Equal("test", variant.Name);
					});
				});

				At(xmb.Elements, xmb.ElementIndexTable, e1.ElementTableIndex, e2 =>
				{
					Assert.Equal("items", e2.Name);
					Assert.Equal(2, e2.ElementCount);
					Assert.Equal(0, e2.AttributeCount);

					At(xmb.Variants, e2.VariantOffset, variant =>
					{
						Assert.Equal(string.Empty, variant.Name);
					});

					At(xmb.Elements, xmb.ElementIndexTable, e2.ElementTableIndex, e3 =>
					{
						Assert.Equal("item", e3.Name);
						Assert.Equal(0, e3.ElementCount);
						Assert.Equal(1, e3.AttributeCount);

						At(xmb.Variants, e3.VariantOffset, variant =>
						{
							Assert.Equal(string.Empty, variant.Name);
						});

						At(xmb.Attributes, xmb.AttributeIndexTable, e3.AttributeTableIndex, attribute =>
						{
							Assert.Equal("name", attribute.Name);
							At(xmb.Variants, attribute.VariantOffset, variant =>
							{
								Assert.Equal("item1", variant.Name);
							});
						});
					});

					At(xmb.Elements, xmb.ElementIndexTable, e2.ElementTableIndex + 1, e3 =>
					{
						Assert.Equal("item", e3.Name);
						Assert.Equal(0, e3.ElementCount);
						Assert.Equal(2, e3.AttributeCount);

						At(xmb.Variants, e3.VariantOffset, variant =>
						{
							Assert.Equal("Value", variant.Name);
						});

						At(xmb.Attributes, xmb.AttributeIndexTable, e3.AttributeTableIndex, attribute =>
						{
							Assert.Equal("name", attribute.Name);
							At(xmb.Variants, attribute.VariantOffset, variant =>
							{
								Assert.Equal("item2", variant.Name);
							});
						});

						At(xmb.Attributes, xmb.AttributeIndexTable, e3.AttributeTableIndex + 1, attribute =>
						{
							Assert.Equal("id", attribute.Name);
							At(xmb.Variants, attribute.VariantOffset, variant =>
							{
								Assert.Equal("1", variant.Name);
							});
						});
					});

				});
			});
		}

		[Theory]
		[InlineData("ctrl_cmn_text_icon.exml")]
		[InlineData("ctrl_exp_normal.exml")]
		[InlineData("ctrl_game_over.exml")]
		[InlineData("initialize1.exml")]
		[InlineData("layout_title_epd.exml")]
		[InlineData("debug_wm.exml")]
		[InlineData("commonai.exml")]
		[InlineData("ocean.exml")]
		[InlineData("arch.exml")]
		[InlineData("cam_shake.exml")]
		[InlineData("cl_qt004_02.exml")]
		[InlineData("debug_quest.exml")]
		[InlineData("map_cl_d01_a_battle.exml")]
		[InlineData("nh_ai_000.exml")]
		public void XmbEqualityTest(string fileName)
		{
			XDocument doc;
			using (var stream = File.OpenRead(fileName))
			{
				doc = new Xmb(new BinaryReader(stream)).Document;
			}

			Compare(doc.Root, new Xmb(doc).Document.Root);
		}

		[Fact]
		public void XmbOptimizationTest()
		{
			XDocument document;
			using (var stream = File.OpenRead("xmb/performance-test1.xml"))
			{
				document = XDocument.Load(stream);
			}

			var xmb = new Xmb(document);
			Assert.Equal(2, xmb.Elements.Count());
			Assert.Equal(2, xmb.Attributes.Count());
			Assert.Equal(3, xmb.Variants.Count());
			Assert.Equal(2, xmb.ElementIndexTable.Count());
			Assert.Equal(2, xmb.AttributeIndexTable.Count());

			At(xmb.ElementIndexTable, 0, x => Assert.Equal(0, x));
			At(xmb.ElementIndexTable, 1, x => Assert.Equal(0, x));
			At(xmb.AttributeIndexTable, 0, x => Assert.Equal(0, x));
			At(xmb.AttributeIndexTable, 1, x => Assert.Equal(1, x));

			At(xmb.Elements, 0, e =>
			{
				Assert.Equal("entry", e.Name);
				Assert.Equal(0, e.ElementTableIndex);
				Assert.Equal(0, e.ElementCount);
				Assert.Equal(1, e.AttributeTableIndex);
				Assert.Equal(1, e.AttributeCount);
			});

			At(xmb.Elements, 1, e =>
			{
				Assert.Equal("not-a-package", e.Name);
				Assert.Equal(0, e.ElementTableIndex);
				Assert.Equal(2, e.ElementCount);
				Assert.Equal(0, e.AttributeTableIndex);
				Assert.Equal(1, e.AttributeCount);
			});
		}

		[Fact]
		public void CollisionTest()
		{
			var h1 = new Xmb.Element()
			{
				AttributeCount = 0,
				AttributeTableIndex = 0,
				ElementCount = 4,
				ElementTableIndex = 4334,
				NameStringOffset = 282,
				VariantOffset = 3
			};

			var h2 = new Xmb.Element()
			{
				AttributeCount = 0,
				AttributeTableIndex = 0,
				ElementCount = 4,
				ElementTableIndex = 12526,
				NameStringOffset = 283,
				VariantOffset = 3
			};

			Assert.NotEqual(h1.GetHashCode(), h2.GetHashCode());
		}

		[Theory]
		[InlineData("ctrl_cmn_text_icon.exml")]
		[InlineData("ctrl_exp_normal.exml")]
		[InlineData("ctrl_game_over.exml")]
		[InlineData("initialize1.exml")]
		[InlineData("layout_title_epd.exml")]
		[InlineData("debug_wm.exml")]
		[InlineData("commonai.exml")]
		[InlineData("ocean.exml")]
		[InlineData("arch.exml")]
		[InlineData("cam_shake.exml")]
		[InlineData("cl_qt004_02.exml")]
		[InlineData("debug_quest.exml")]
		[InlineData("map_cl_d01_a_battle.exml")]
		[InlineData("nh_ai_000.exml")]
		public void XmbPerformanceTest(string fileName)
		{
			Xmb xmbOrigin;
			using (var stream = File.OpenRead(fileName))
			{
				xmbOrigin = new Xmb(new BinaryReader(stream));
			}

			Compare(xmbOrigin, new Xmb(xmbOrigin.Document));
		}

		[Theory]
		[InlineData(null, Xmb.ValueType.Unknown)]
		[InlineData("", Xmb.ValueType.Unknown)]
		[InlineData("false", Xmb.ValueType.Bool)]
		[InlineData("true", Xmb.ValueType.Bool)]
		[InlineData("False", Xmb.ValueType.Bool)]
		[InlineData("True", Xmb.ValueType.Bool)]
		[InlineData("0", Xmb.ValueType.Unsigned)]
		[InlineData("1", Xmb.ValueType.Unsigned)]
		[InlineData("2", Xmb.ValueType.Unsigned)]
		[InlineData("-1", Xmb.ValueType.Signed)]
		[InlineData("0.0", Xmb.ValueType.Float)]
		[InlineData("0.1", Xmb.ValueType.Float)]
		[InlineData("1.0", Xmb.ValueType.Float)]
		[InlineData("1.1", Xmb.ValueType.Float)]
		[InlineData("-1.1", Xmb.ValueType.Float)]
		[InlineData("0,0", Xmb.ValueType.Float2)]
		[InlineData("0,0,0", Xmb.ValueType.Float3)]
		[InlineData("0,0,0,0", Xmb.ValueType.Float4)]
		[InlineData("-1,0", Xmb.ValueType.Float2)]
		[InlineData("1.1,0", Xmb.ValueType.Float2)]
		[InlineData("ffxv.xmb", Xmb.ValueType.Unknown)]
		[InlineData("THIS_IS_AN_ENUM", Xmb.ValueType.Unknown)]
		[InlineData("-8.651422E-06", Xmb.ValueType.Unknown)]
		public void GuessTypeFromValueTest(string value, Xmb.ValueType expected)
		{
			Assert.Equal(expected, Xmb.GuessTypeFromValue(value));
		}

		private void At<T>(IEnumerable<T> items, int index, Action<T> action)
		{
			action(items.ElementAt(index));
		}

		private void At<T>(IEnumerable<T> items, IEnumerable<int> indicies, int index, Action<T> action)
		{
			action(items.ElementAt(indicies.ElementAt(index)));
		}

		private void Compare(XElement[] expected, XElement[] actual)
		{
			Assert.Equal(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Compare(expected[i], actual[i]);
			}
		}

		private void Compare(XElement expected, XElement actual)
		{
			Assert.Equal(expected.Name, actual.Name);
			Compare(expected.Attributes().ToArray(), actual.Attributes().ToArray());
			Compare(expected.Elements().ToArray(), actual.Elements().ToArray());
			Assert.Equal(expected.Value, actual.Value);
		}

		private void Compare(XAttribute[] expected, XAttribute[] actual)
		{
			Assert.Equal(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Compare(expected[i], actual[i]);
			}
		}

		private void Compare(XAttribute expected, XAttribute actual)
		{
			Assert.Equal(expected.Name, actual.Name);
			Assert.Equal(expected.Value, actual.Value);
		}


		private void Compare(Xmb expected, Xmb actual)
		{
			AssertEqual(expected.Elements, actual.Elements, AssertEqual);
			AssertEqual(expected.Attributes, actual.Attributes, AssertEqual);
			AssertEqual(expected.Variants, actual.Variants, AssertEqual);
			AssertEqual(expected.ElementIndexTable, actual.ElementIndexTable, AssertEqual);
			AssertEqual(expected.AttributeIndexTable, actual.AttributeIndexTable, AssertEqual);
			AssertEqual(expected.StringData, actual.StringData, AssertEqual);
		}

		private static string GetElementFromVariant(Xmb.Element[] elements, List<Xmb.Attribute> attributes, List<Xmb.Variant> variants, List<int> attributeIndexes, int variantOffset)
		{
			return GetElementFromVariant(elements.ToList(), attributes, variants, attributeIndexes, variantOffset);
		}

		private static string GetElementFromVariant(List<Xmb.Element> elements, List<Xmb.Attribute> attributes, List<Xmb.Variant> variants, List<int> attributeIndexes, int variantOffset)
		{
			var e = elements.FirstOrDefault(x => x.VariantOffset == variantOffset);
			if (e != null)
			{
				var strElement = $"{e.Name}: {variants[e.VariantOffset].Name} (type: {variants[e.VariantOffset].Type})\n";
				for (int j = 0; j < e.AttributeCount; j++)
				{
					var a = attributes[attributeIndexes[e.AttributeTableIndex + j]];
					strElement += $"\t{a.Name} = {variants[a.VariantOffset].Name}\n";
				}

				return strElement;
			}

			return null;
		}

		private void AssertEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, Action<T, T> funcCompare)
		{
			var va = expected.ToArray();
			var vb = actual.ToArray();

			Assert.Equal(va.Length, vb.Length);
			for (int i = 0; i < va.Length; i++)
			{
				funcCompare(va[i], vb[i]);
			}
		}

		private void AssertEqual(Xmb.Element expected, Xmb.Element actual)
		{
			Assert.Equal(expected.AttributeTableIndex, actual.AttributeTableIndex);
			Assert.Equal(expected.AttributeCount, actual.AttributeCount);
			Assert.Equal(expected.ElementTableIndex, actual.ElementTableIndex);
			Assert.Equal(expected.ElementCount, actual.ElementCount);
			Assert.Equal(expected.VariantOffset, actual.VariantOffset);
			Assert.Equal(expected.NameStringOffset, actual.NameStringOffset);
			Assert.Equal(expected.Name, actual.Name);
		}

		private void AssertEqual(Xmb.Attribute expected, Xmb.Attribute actual)
		{
			Assert.Equal(expected.VariantOffset, actual.VariantOffset);
			Assert.Equal(expected.NameStringOffset, actual.NameStringOffset);
			Assert.Equal(expected.Name, actual.Name);
		}

		private void AssertEqual(Xmb.Variant expected, Xmb.Variant actual)
		{
			Assert.Equal(expected.Type, actual.Type);
			Assert.Equal(expected.NameStringOffset, actual.NameStringOffset);
			Assert.Equal(expected.Value1, actual.Value1);
			Assert.Equal(expected.Value2, actual.Value2);
			Assert.Equal(expected.Value3, actual.Value3);
			Assert.Equal(expected.Value4, actual.Value4);
			Assert.Equal(expected.Name, actual.Name);
		}

		private void AssertEqual(byte expected, byte actual)
		{
			Assert.Equal(expected, actual);
		}

		private void AssertEqual(int expected, int actual)
		{
			Assert.Equal(expected, actual);
		}
	}
}
