using System.Collections.Generic;
using SQEX.Ebony;
using SQEX.Ebony.Framework.Node;
using SQEX.Ebony.Framework.Sequence;

namespace Black.Sequence.Event
{
	public class SequenceRequestEvent2 : SequenceNode
	{
		public GraphTriggerInputPin In { get; set; }

		public GraphTriggerOutputPin Out { get; set; }

		public long? EventId { get; set; }

		public string EventString { get; set; }

		public EventType EventType { get; set; }

		public EventType EventTypeLast { get; set; }

		public GraphVariableInputPin CausedActor { get; set; }

		public GraphVariableInputPin HandlerActor { get; set; }

		public bool PrintLog { get; set; }
	}
}
