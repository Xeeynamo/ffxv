using System.Collections.Generic;

namespace SQEX.Ebony.Framework.Sequence.Tray
{
	public class SequenceTray : SequenceNode
	{
		public ICollection<EntityReference> Nodes { get; set; }

		public string TrayName { get; set; }

		public Color HeaderColorUserDefine { get; set; }

		public Color BodyColorUserDefine { get; set; }
		
		// TODO
		public string Groups { get; set; }

		public TrayType TrayType { get; set; }

		public double PositionWidth { get; set; }

		public double PositionHeight { get; set; }

		public bool IsOpen { get; set; }
	}
}
