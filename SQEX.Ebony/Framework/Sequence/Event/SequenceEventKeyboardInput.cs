namespace SQEX.Ebony.Framework.Sequence.Event
{
	public class SequenceEventKeyboardInput : SequenceNode
	{
		public SequenceContainerOutPin Enable { get; set; }

		public SequenceContainerOutPin Disable { get; set; }

		public SequenceContainerOutPin Push { get; set; }

		public SequenceContainerOutPin Repeat { get; set; }

		public SequenceContainerOutPin Released { get; set; }

		public bool BIsNeedShiftPress { get; set; }

		public bool BIsNeedCtrlPress { get; set; }

		public bool BIsNeedAltPress { get; set; }

		public string InputKey { get; set; }
	}
}
