using System.Collections.Generic;

namespace SQEX.Ebony.Framework.Entity
{
	public class EntityPackage
	{
		public string PresetFilePath { get; set; }

		public ICollection<EntityReference> Entities { get; set; }

		public bool HasTransform { get; set; }

		public Float4 Position { get; set; }

		public Float4 Rotation { get; set; }

		public float Scaling { get; set; }

		public bool CanManipulate { get; set; }

		public string SourcePath { get; set; }

		public string Name { get; set; }

		public bool IsTemplateTraySourceReference { get; set; }

		public bool IsShared { get; set; }

		public bool StartupLoad { get; set; }
	}
}
