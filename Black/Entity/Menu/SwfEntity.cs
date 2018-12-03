using SQEX.Ebony;

namespace Black.Entity.Menu
{
	public enum DrawType
	{
		SWFDRAW_TYPE_2D = 0
	}

	public enum DrawLayer
	{
		SWFDRAW_LAYER_DEPTH_1 = 4
	}

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
