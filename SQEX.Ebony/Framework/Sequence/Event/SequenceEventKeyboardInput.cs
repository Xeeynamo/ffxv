using System;
using System.Collections.Generic;
using System.Text;
using SQEX.Ebony.Framework.Node;

namespace SQEX.Ebony.Framework.Sequence.Event
{
	public class SequenceEventKeyboardInput
	{
		public ICollection<GraphVariableInputPin> RefInPorts { get; set; }

		public ICollection<GraphVariableOutputPin> RefOutPorts { get; set; }

		public string Comment { get; set; }

		public ICollection<GraphTriggerInputPin> TriInPorts { get; set; }

		public ICollection<GraphTriggerOutputPin> TriOutPorts { get; set; }

		public SequenceContainerOutPin Enable { get; set; }

		public SequenceContainerOutPin Disable { get; set; }

		public SequenceContainerOutPin Push { get; set; }

		public SequenceContainerOutPin Repeat { get; set; }

		public SequenceContainerOutPin Released { get; set; }

		public bool BIsNeedShiftPress { get; set; }

		public bool BIsNeedCtrlPress { get; set; }

		public bool BIsNeedAltPress { get; set; }

		public string InputKey { get; set; }

		public double PositionX { get; set; }

		public double PositionY { get; set; }
	}
}
