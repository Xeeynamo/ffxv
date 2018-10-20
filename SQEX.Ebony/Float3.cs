using System;

namespace SQEX.Ebony
{
	public class Float3
	{
		public float x, y, z;

		public override string ToString() => $"{x},{y},{z}";

		public static Float3 FromString(string str)
		{
			var value = new Float3();
			var strs = str.Split(',');
			if (strs.Length >= 4)
			{
				value.x = float.Parse(strs[0]);
				value.y = float.Parse(strs[1]);
				value.z = float.Parse(strs[2]);
			}
			else
			{
				throw new ArgumentException($"{str} is not a valid {typeof(Float3).Name} format");
			}

			return value;
		}
	}
}
