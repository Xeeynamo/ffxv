namespace Black.Entity.Menu
{
	public class SwfDefEntity
	{
		public string PresetFilePath { get; set; }

		public string FilePath { get; set; }

		public SwfType SwfType { get; set; }

		public DirectionType DirectionType { get; set; }

		public float DefaultRatio { get; set; }

		public float SpeedRation { get; set; }

		public bool HasPixelInfo { get; set; }

		public int LengthPixel { get; set; }

		public int OffsetPixel { get; set; }
	}
}
