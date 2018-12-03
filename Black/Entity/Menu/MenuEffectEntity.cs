using System;
using System.Collections.Generic;
using System.Text;
using SQEX.Ebony;

namespace Black.Entity.Menu
{
	public class MenuEffectEntity
	{
		public string PresetFilePath { get; set; }

		public Float4 Position { get; set; }

		public Float4 Rotation { get; set; }

		public string EntitySearchLabelId { get; set; }

		public DrawType DrawType { get; set; }

		public DrawLayer DrawLayer { get; set; }

		public string VfxPath { get; set; }

		public bool IsAutoPlay { get; set; }
	}
}
