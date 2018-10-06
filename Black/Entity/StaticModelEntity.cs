using SQEX.Ebony;

namespace Black.Entity
{
	public class StaticModelEntity
	{
		public string PresetFilePath { get; set; }

		public Float4 Position { get; set; }

		public Float4 Rotation { get; set; }

		public object EntitySearchLabelId { get; set; }

		public string SourcePath { get; set; }

		public float Scaling { get; set; }

		public bool Visible { get; set; }

		public bool CasterShadow { get; set; }

		public bool ReceiveShadow { get; set; }

		public bool PreComputedShadow { get; set; }

		public bool UseMeshCollision { get; set; }

		public string MeshCollision { get; set; }

		public bool UseMeshCollisionMovingTile { get; set; }

		public bool UseMeshCollisionAirMovingTIle { get; set; }
	}
}
