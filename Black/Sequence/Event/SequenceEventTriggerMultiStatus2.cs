using SQEX.Ebony;
using SQEX.Ebony.Framework.Node;
using SQEX.Ebony.Framework.Sequence;

namespace Black.Sequence.Event
{
	public class SequenceEventTriggerMultiStatus2 : SequenceNode
	{
		public GraphVariableInputPin TriggerPointPin { get; set; }

		public GraphTriggerInputPin RemoteEvent { get; set; }

		public GraphTriggerInputPin Touch { get; set; }

		public GraphTriggerInputPin UnTouch { get; set; }

		public GraphTriggerInputPin AttackIn { get; set; }

		public GraphTriggerInputPin AttackOut { get; set; }

		public GraphVariableInputPin CausedActor { get; set; }

		public GraphVariableInputPin TriggerActor { get; set; }

		public long? EventId { get; set; }

		public string EventString { get; set; }

		public EventType EventType { get; set; }

		public EventType EventTypeLast { get; set; }

		public bool CanCatchSameTimeEvents { get; set; }
	}
}
