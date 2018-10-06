using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace FFXV.Services.Tests
{
	public class XmbTest
	{
		[Theory]
		[InlineData("ctrl_cmn_text_icon.exml")]
		[InlineData("ctrl_exp_normal.exml")]
		[InlineData("ctrl_game_over.exml")]
		[InlineData("initialize1.exml")]
		[InlineData("layout_title_epd.exml")]
		[InlineData("debug_wm.exml")]
		[InlineData("commonai.exml")]
		[InlineData("ocean.exml")]
		public void XmbEqualityTest(string fileName)
		{
			var fileIn = File.OpenRead(fileName);
			var mem = new MemoryStream();

			var xml = Xmb.Open(fileIn);
			Xmb.Save(mem, xml);
			mem.Position = 0;

			mem.Position = 0;
			var xml2 = Xmb.Open(mem);
			Compare(xml, xml2);

			mem.Dispose();
			fileIn.Dispose();
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
		public void XmbPerformanceTest(string fileName)
		{
			var fileIn = File.OpenRead(fileName);
			var mem = new MemoryStream();

			var xml = Xmb.Open(fileIn);
			Xmb.Save(mem, xml);
			mem.Position = 0;

			fileIn.Position = 0;
			Xmb.Compare(xml, fileIn);

			mem.Dispose();
			fileIn.Dispose();
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
	}
}
