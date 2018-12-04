using System.Collections.Generic;
using SQEX.Ebony.Framework.Node;
using SQEX.Ebony.Framework.Sequence;

namespace Black.Sequence.Menu
{
	public class SequenceActionStatusMenuVisible : SequenceNode
	{
		public GraphTriggerInputPin Show { get; set; }

		public GraphTriggerInputPin Hide { get; set; }

		public GraphTriggerOutputPin Out { get; set; }
	}
}
