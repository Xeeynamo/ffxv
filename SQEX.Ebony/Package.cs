using System.Collections.Generic;

namespace SQEX.Ebony
{
	public class Package
	{
		public string Name { get; set; }

		public ICollection<Object> Objects { get; set; }
	}
}
