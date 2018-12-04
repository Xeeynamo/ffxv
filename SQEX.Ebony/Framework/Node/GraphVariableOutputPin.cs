using System.Collections.Generic;

namespace SQEX.Ebony.Framework.Node
{
	public class GraphVariableOutputPin
	{
		public string PinName { get; set; }

		public string Name { get; set; }

		public bool IsBrowsable { get; set; }

		public ICollection<EntityReference> Connections { get; set; }

		public string PinValueType { get; set; }
	}
}
