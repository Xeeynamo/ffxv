using SQEX.Ebony.Framework.Node;

namespace SQEX.Ebony.Framework.Sequence.Action
{
	public class SequenceActionSetVariableWithProperty : SequenceNode
	{
		public GraphTriggerInputPin In { get; set; }

		public GraphTriggerOutputPin Out { get; set; }

		public GraphVariableInputPin VarGet { get; set; }

		public GraphVariableOutputPin VarSet { get; set; }

		public SequenceVariableType Type { get; set; }

		public SequenceVariableType TypeLast { get; set; }

		public bool BoolValue { get; set; }

		public int IntValue { get; set; }

		public float FloatValue { get; set; }

		public Float4 VectorValue { get; set; }

		public string StringValue { get; set; }

		public bool IsRandom { get; set; }

		public float RandomMin { get; set; }

		public float RandomMax { get; set; }
	}
}
