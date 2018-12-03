using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace FFXV.Services.Tests
{
	public class PackageTests
	{
		[Fact]
		public void PackageDeserialization()
		{
			XDocument doc;
			using (var stream = File.OpenRead("ocean.exml"))
			{
				doc = new Xmb(new BinaryReader(stream)).Document;
			}

			var pkg = PackageService.Open(doc.Root);
			Assert.Equal("ocean", pkg.Name);

			Assert.NotNull(pkg.Objects);
			Assert.True(pkg.Objects.Count() == 6);

			Assert.IsType<SQEX.Ebony.Object>(pkg.Objects.First());
			if (pkg.Objects.First() is SQEX.Ebony.Object obj)
			{
				Assert.Equal(0, obj.ObjectIndex);
				Assert.Equal("ocean", obj.Name);
				Assert.Equal("SQEX.Ebony.Framework.Entity.EntityPackageReference", obj.Type);
				Assert.Equal("", obj.Path);
				Assert.True(obj.Checked);

				Assert.NotNull(obj.Item);
				Assert.IsType<SQEX.Ebony.Framework.Entity.EntityPackageReference>(obj.Item);
				if (obj.Item is SQEX.Ebony.Framework.Entity.EntityPackageReference entityPackage)
				{
					Assert.Equal("environment/world/ebex/ocean.ebex", entityPackage.SourcePath);
					Assert.Equal(0.0f, entityPackage.Position.x);
					Assert.Equal(1.0f, entityPackage.Position.w);

					Assert.Equal(5, entityPackage.Entities.Count);
				}
			}

			Assert.IsType<SQEX.Ebony.Object>(pkg.Objects.Skip(1).First());
			if (pkg.Objects.Skip(1).First() is SQEX.Ebony.Object obj2)
			{
				Assert.Equal(1, obj2.ObjectIndex);
				Assert.Equal("sea00", obj2.Name);
				Assert.Equal("entities_", obj2.OwnerPath);
				Assert.Equal("entities_.sea00", obj2.Path);
				Assert.Equal("Black.Entity.StaticModelEntity", obj2.Type);

				Assert.NotNull(obj2.Item);
				Assert.IsType<Black.Entity.StaticModelEntity>(obj2.Item);
				if (obj2.Item is Black.Entity.StaticModelEntity staticModelEntity)
				{
					Assert.Equal(-2416.375, staticModelEntity.Position.x, 3);
					Assert.Equal(0, staticModelEntity.Position.y, 3);
					Assert.Equal(-3046.105, staticModelEntity.Position.z, 3);
					Assert.Equal(1, staticModelEntity.Position.w, 3);
					Assert.Equal("environment/world/props/pond_water01/models/pond_water01_A.gmdl", staticModelEntity.SourcePath);
					Assert.True(staticModelEntity.Visible);
				}
			}
		}

		[Theory]
		[InlineData("ctrl_cmn_text_icon.exml")]
		[InlineData("ctrl_exp_normal.exml")]
		[InlineData("ctrl_game_over.exml")]
		[InlineData("initialize1.exml")]
		//[InlineData("layout_title_epd.exml")]
		//[InlineData("debug_wm.exml")]
		//[InlineData("commonai.exml")]
		[InlineData("ocean.exml")]
		public void PackageCheckTypesOnDeserialization(string fileName)
		{
			XDocument doc;
			using (var stream = File.OpenRead(fileName))
			{
				doc = new Xmb(new BinaryReader(stream)).Document;
			}

			var pkg = PackageService.Open(doc.Root);
		}
	}
}
