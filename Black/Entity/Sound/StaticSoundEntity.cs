using SQEX.Ebony;

namespace Black.Entity.Sound
{
	public class StaticSoundEntity
	{
		public string PresetFilePath { get; set; }

		public Float4 Position { get; set; }

		public Float4 Rotation { get; set; }

		public string EntitySearchLabelId { get; set; }

		public bool Streaming { get; set; }

		public float SoundVolume { get; set; }

		public float FadeTimeVol { get; set; }

		public float SoundPitch { get; set; }

		public float FadeTimePitch { get; set; }

		public float SoundFadeInTime { get; set; }

		public float SoundFadeOutTime { get; set; }

		public bool SoundVisibleFlag { get; set; }

		public bool SoundLoopFlag { get; set; }

		public string IdxFilePath { get; set; }

		public int SoundNumber { get; set; }

		public bool UsePrimitiveFlag { get; set; }

		public float Scaling { get; set; }

		public Float4 ScalingVec { get; set; }

		public SoundObjType SoundObjType { get; set; }

		public float AudibleRange { get; set; }

		public float InnerRange { get; set; }

		// TODO
		public string PositionEntityList { get; set; }

		public float Pan { get; set; }

		public float FrPan { get; set; }

		public float UdPan { get; set; }

		public float FadeTimePan { get; set; }

		public float InteriorFactor { get; set; }

		public bool UseNavSoundFlag { get; set; }
	}
}
