using System.Collections.Generic;
using SQEX.Ebony.Framework.Node;

namespace SQEX.Ebony.Framework.Sequence
{
	public class SequenceNode
	{
		public ICollection<GraphVariableInputPin> RefInPorts { get; set; }

		public ICollection<GraphVariableOutputPin> RefOutPorts { get; set; }

		public string Comment { get; set; }

		public ICollection<GraphTriggerInputPin> TriInPorts { get; set; }

		public ICollection<GraphTriggerOutputPin> TriOutPorts { get; set; }

		public double PositionX { get; set; }

		public double PositionY { get; set; }
	}
}
