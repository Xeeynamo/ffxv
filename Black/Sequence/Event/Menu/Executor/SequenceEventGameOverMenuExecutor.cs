using SQEX.Ebony.Framework.Sequence;

namespace Black.Sequence.Event.Menu.Executor
{
	public class SequenceEventGameOverMenuExecutor
	{
		// TODO
		public string RefInPorts { get; set; }

		// TODO
		public string RefOutPorts { get; set; }

		public string Comment { get; set; }

		// TODO
		public string TriInPorts { get; set; }

		// TODO
		public string TriOutPorts { get; set; }

		public SequenceContainerOutPin OutPin { get; set; }

		public SequenceContainerPin Pin0 { get; set; }
		public SequenceContainerPin Pin1 { get; set; }
		public SequenceContainerPin Pin2 { get; set; }
		public SequenceContainerPin Pin3 { get; set; }
		public SequenceContainerPin Pin4 { get; set; }
		public SequenceContainerPin Pin5 { get; set; }
		public SequenceContainerPin Pin6 { get; set; }
		public SequenceContainerPin Pin7 { get; set; }

		public double PositionX { get; set; }

		public double PositionY { get; set; }
	}
}
