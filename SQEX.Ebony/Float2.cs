using System;

namespace SQEX.Ebony
{
	public class Float2
	{
		public float x, y;

		public override string ToString() => $"{x},{y}";

		public static Float2 FromString(string str)
		{
			var value = new Float2();
			var strs = str.Split(',');
			if (strs.Length >= 2)
			{
				value.x = float.Parse(strs[0]);
				value.y = float.Parse(strs[1]);
			}
			else
			{
				throw new ArgumentException($"{str} is not a valid {typeof(Float2).Name} format");
			}

			return value;
		}
	}
}
