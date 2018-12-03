using SQEX.Ebony.Framework.Sequence;

namespace Black.Sequence.Variable
{
	public class SequenceVariableMenuEffectEntity
	{
		// TODO
		public string RefInPorts { get; set; }

		// TODO
		public string RefOutPorts { get; set; }

		public string Comment { get; set; }

		public SequenceContainerPin OutValue { get; set; }

		public string EntityPointer { get; set; }

		public double PositionX { get; set; }

		public double PositionY { get; set; }
	}
}
