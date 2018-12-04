using System.Collections.Generic;

namespace SQEX.Ebony.Framework.Node
{
	public class GraphTriggerOutputPin
	{
		public string PinName { get; set; }

		public string Name { get; set; }

		public bool IsBrowsable { get; set; }

		public ICollection<EntityReference> Connections { get; set; }

		public DelayType DelayType { get; set; }

		public float DelayTime { get; set; }

		public float DelayMaxType { get; set; }

		public PinType PinType { get; set; }
	}
}
