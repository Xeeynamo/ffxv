using System.Collections.Generic;
using SQEX.Ebony.Framework.Node;

namespace SQEX.Ebony.Framework.Sequence.Tray
{
	public class SequenceTray
	{
		public ICollection<GraphVariableInputPin> RefInPorts { get; set; }

		public ICollection<GraphVariableOutputPin> RefOutPorts { get; set; }

		public string Comment { get; set; }
		
		public ICollection<GraphTriggerInputPin> TriInPorts { get; set; }

		public ICollection<GraphTriggerOutputPin> TriOutPorts { get; set; }

		public ICollection<EntityReference> Nodes { get; set; }

		public string TrayName { get; set; }

		public Color HeaderColorUserDefine { get; set; }

		public Color BodyColorUserDefine { get; set; }
		
		// TODO
		public string Groups { get; set; }

		public TrayType TrayType { get; set; }

		public double PositionX { get; set; }

		public double PositionY { get; set; }

		public double PositionWidth { get; set; }

		public double PositionHeight { get; set; }

		public bool IsOpen { get; set; }
	}
}
