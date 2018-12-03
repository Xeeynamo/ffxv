namespace Black.Entity.Menu
{
	public class MenuPackage
	{
		public string SourcePath { get; set; }

		public bool IsTemplateTraySourceReference { get; set; }

		// TODO is this an enum??
		public MenuPackageType MenuPackageType { get; set; }
	}
}
