using System.Collections.Generic;
using SQEX.Ebony;

namespace Black.Entity.Area
{
	public class MapPackage
	{
		public string PresetFilePath { get; set; }

		public IEnumerable<EntityReference> Entities { get; set; }

		public bool HasTransform { get; set; }

		public Float4 Position { get; set; }

		public Float4 Rotation { get; set; }

		public float Scaling { get; set; }

		public bool CanManipulate { get; set; }

		public string SourcePath { get; set; }

		public string Name { get; set; }

		public bool IsTemplateTraySourceReference_ { get; set; }

		public bool IsShared { get; set; }

		public bool StartupLoad { get; set; }

		// TODO loadedObjects_
		// TODO loadedObjectNames_
		// TODO loadedObjectPaths_
		// TODO entityPackageSharedChildPathList_

		public Float4 Min { get; set; }

		public Float4 Max { get; set; }

		public string ParentPackagePath { get; set; }
	}
}
