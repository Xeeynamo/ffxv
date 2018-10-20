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
			using (var stream = File.OpenRead("cam_quest.exml"))
			{
				doc = new Xmb2(new BinaryReader(stream)).Document;
			}
		}
	}
}
