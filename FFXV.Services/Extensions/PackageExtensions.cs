using System.Collections.Generic;
using FFXV.Models;
using SQEX.Ebony;

namespace FFXV.Services.Extensions
{
	public static class PackageExtensions
	{
		public static PackageObject ToPackageObject(this Package package)
		{
			return new PackageObject
			{
				Package = package,
				Object = package.Objects[0]
			};
		}
	}
}
