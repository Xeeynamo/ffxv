using System;

namespace SQEX.Ebony
{
	public class Float4
	{
		public float x, y, z, w;

		public static Float4 FromString(string str)
		{
			var value = new Float4();
			var strs = str.Split(',');
			if (strs.Length >= 4)
			{
				value.x = float.Parse(strs[0]);
				value.y = float.Parse(strs[1]);
				value.z = float.Parse(strs[2]);
				value.w = float.Parse(strs[3]);
			}
			else
			{
				throw new ArgumentException($"{str} is not a valid {typeof(Float4).Name} format");
			}

			return value;
		}
	}
}
