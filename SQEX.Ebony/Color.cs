using System;

namespace SQEX.Ebony
{
	public class Color
	{
		public float r, g, b, a;

		public override string ToString() => $"{r},{g},{b},{a}";

		public static Color FromString(string str)
		{
			var value = new Color();
			var strs = str.Split(',');
			if (strs.Length >= 4)
			{
				value.r = float.Parse(strs[0]);
				value.g = float.Parse(strs[1]);
				value.b = float.Parse(strs[2]);
				value.a = float.Parse(strs[3]);
			}
			else
			{
				throw new ArgumentException($"{str} is not a valid {typeof(Color).Name} format");
			}

			return value;
		}
	}
}
