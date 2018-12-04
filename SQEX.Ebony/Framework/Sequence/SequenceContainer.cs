using System.Collections.Generic;

namespace SQEX.Ebony.Framework.Sequence
{
	public class SequenceContainer
	{
		public string PresetFilePath { get; set; }

		public ICollection<EntityReference> Nodes { get; set; }

		// TODO
		public string Groups { get; set; }

		public ICollection<EntityReference> SelectedNodes { get; set; }

		public float LastCenterX { get; set; }

		public float LastCenterY { get; set; }

		public bool bIsParallelExecution { get; set; }

		public bool bIsEntityScript { get; set; }

		public uint UpdateStates { get; set; }

		public bool bIsPrefabTopSequence { get; set; }
	}

	public class SequenceContainerPin
	{
		public string PinName { get; set; }

		public string Name { get; set; }

		public bool IsBrowsable { get; set; }

		public ICollection<EntityReference>	Connections { get; set; }

		public string PinValueType { get; set; }
	}

	public class SequenceContainerOutPin : SequenceContainerPin
	{
		public DelayType DelayType { get; set; }

		public float DelayTime { get; set; }

		public float DelayMaxTime { get; set; }

		public PinType PinType { get; set; }
	}
}
