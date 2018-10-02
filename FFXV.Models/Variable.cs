using System.Collections.Generic;

namespace FFXV.Models
{
	public class Variable
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public int Unk2 { get; set; }
		public List<Sub> Sub { get; set; }
	}

	public class Sub
	{
		public int DebugIndex { get; set; }
		public string Decl { get; set; }
		public string Type { get; set; }
	}
}
