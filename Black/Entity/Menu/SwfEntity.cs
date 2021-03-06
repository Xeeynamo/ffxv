﻿using SQEX.Ebony;

namespace Black.Entity.Menu
{
	public class SwfEntity
	{
		public string PresetFilePath { get; set; }

		public Float4 Position { get; set; }

		public Float4 Rotation { get; set; }

		public long? EntitySearchLabelId { get; set; }

		public string SwfEntry { get; set; }

		public DrawType DrawType { get; set; }

		public DrawLayer DrawLayer { get; set; }

		public bool IsAutoPlay { get; set; }
	}
}
