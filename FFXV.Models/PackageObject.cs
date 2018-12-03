using System.Collections.Generic;
using System.Linq;
using SQEX.Ebony;

namespace FFXV.Models
{
	public class PackageObject
	{
		public Package Package { get; set; }

		public Object Object { get; set; }

		public PackageObject Parent
		{
			get
			{
				if (!Object.OwnerIndex.HasValue)
					return null;

				return new PackageObject
				{
					Package = Package,
					Object = Package.Objects[Object.ObjectIndex]
				};
			}
		}

		public IEnumerable<PackageObject> Children => Package.Objects
			.Where(x => x.OwnerIndex == Object.ObjectIndex)
			.Select(x => new PackageObject
			{
				Package = Package,
				Object = Package.Objects[x.ObjectIndex]
			});

		public object Item => Object.Item;

		public override string ToString() => FirstNotNullOrEmpty(Object.Path, Object.Name, $"{Package.Name}[{Object.ObjectIndex}]");

		private static string FirstNotNullOrEmpty(params string[] strs)
		{
			return strs.FirstOrDefault(x => !string.IsNullOrEmpty(x));
		}
	}
}
