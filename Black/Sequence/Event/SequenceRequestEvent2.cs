using System.Collections.Generic;
using SQEX.Ebony;
using SQEX.Ebony.Framework.Node;

namespace Black.Sequence.Event
{
	public class SequenceRequestEvent2
	{
		public ICollection<GraphVariableInputPin> RefInPorts { get; set; }

		public ICollection<GraphVariableOutputPin> RefOutPorts { get; set; }

		public string Comment { get; set; }

		public ICollection<GraphTriggerInputPin> TriInPorts { get; set; }

		public ICollection<GraphTriggerOutputPin> TriOutPorts { get; set; }

		public GraphTriggerInputPin In { get; set; }

		public GraphTriggerOutputPin Out { get; set; }

		public long? EventId { get; set; }

		public string EventString { get; set; }

		public EventType EventType { get; set; }

		public EventType EventTypeLast { get; set; }

		public GraphVariableInputPin CausedActor { get; set; }

		public GraphVariableInputPin HandlerActor { get; set; }

		public bool PrintLog { get; set; }

		public double PositionX { get; set; }

		public double PositionY { get; set; }
	}
}
