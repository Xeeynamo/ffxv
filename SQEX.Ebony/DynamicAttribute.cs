using System;

namespace SQEX.Ebony
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DynamicAttribute : Attribute
	{
		public bool IsDynamic { get; }

		public DynamicAttribute(bool isDynamic = true)
		{
			IsDynamic = isDynamic;
		}

		public static bool IsObjectDynamic(Type type)
		{
			var attribute = GetCustomAttribute(type, typeof(DynamicAttribute)) as DynamicAttribute;
			return attribute?.IsDynamic ?? false;
		}
	}
}
