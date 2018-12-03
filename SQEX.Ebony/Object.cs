namespace SQEX.Ebony
{
	public class Object
	{
		public int ObjectIndex { get; set; }

		public string Name { get; set; }

		public string Type { get; set; }

		public string Path { get; set; }

		public bool Checked { get; set; }

		public string Owner { get; set; }

		public int? OwnerIndex { get; set; }

		public string OwnerPath { get; set; }

		public object Item { get; set; }
	}
}
