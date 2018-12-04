using SQEX.Ebony.Framework.Sequence;

namespace Black.Sequence.Event.Menu.Executor
{
	public class SequenceEventNowLoadingExecutor : SequenceNode
	{
		public SequenceContainerOutPin OutPin { get; set; }

		public SequenceContainerPin Pin0 { get; set; }
		public SequenceContainerPin Pin1 { get; set; }
		public SequenceContainerPin Pin2 { get; set; }
		public SequenceContainerPin Pin3 { get; set; }
		public SequenceContainerPin Pin4 { get; set; }
		public SequenceContainerPin Pin5 { get; set; }
		public SequenceContainerPin Pin6 { get; set; }
		public SequenceContainerPin Pin7 { get; set; }

		// TODO missing fixid
		public string NowLoadingMsgId { get; set; }

		// TODO missing fixid
		public string HeadLineMsgId { get; set; }

		// TODO missing fixid
		public string BodyMsgId { get; set; }

		public int MaxloadedMegaByte { get; set; }
	}
}
