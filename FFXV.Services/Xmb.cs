using FFXV.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FFXV.Services
{
	public class Xmb
	{
		public enum ValueType
		{
			Unknown = 0,
			Bool = 1,
			Signed = 2,
			Unsigned = 3,
			Float = 4,
			Vector4 = 8,
		}

		private class HeaderEntry
		{
			public int Offset { get; set; }
			public int Count { get; set; }

			public static HeaderEntry Read(BinaryReader reader)
			{
				return new HeaderEntry()
				{
					Offset = reader.ReadInt32(),
					Count = reader.ReadInt32(),
				};
			}
		}

		private class Header
		{
			public uint MagicCode { get; set; }
			public uint RESERVED { get; set; }
			public HeaderEntry Table1 { get; set; } // Variables?
			public HeaderEntry Table2 { get; set; }
			public HeaderEntry TableNames { get; set; }
			public HeaderEntry Table4 { get; set; }
			public HeaderEntry Table5 { get; set; }
			public HeaderEntry Table6 { get; set; }
			public int Unk38 { get; set; }
			public int Reserved3c { get; set; }
			public int Reserved40 { get; set; }
			public int Reserved44 { get; set; }
			public int Reserved48 { get; set; }

			public static Header Read(BinaryReader reader)
			{
				return new Header()
				{
					MagicCode = reader.ReadUInt32(),
					RESERVED = reader.ReadUInt32(),
					Table1 = HeaderEntry.Read(reader),
					Table2 = HeaderEntry.Read(reader),
					TableNames = HeaderEntry.Read(reader),
					Table4 = HeaderEntry.Read(reader),
					Table5 = HeaderEntry.Read(reader),
					Table6 = HeaderEntry.Read(reader),
					Unk38 = reader.ReadInt32(),
					Reserved3c = reader.ReadInt32(),
					Reserved40 = reader.ReadInt32(),
					Reserved44 = reader.ReadInt32(),
					Reserved48 = reader.ReadInt32(),
				};
			}
		}

		public class Table1
		{
			public int Zero00 { get; set; }
			public int Zero04 { get; set; }
			public int Table5 { get; set; }
			public int ElementsCount { get; set; }
			public int Table4 { get; set; }
			public int Unk14 { get; set; }
			public int Name { get; set; }
			public int Table6 { get; set; }

			public string Str { get; set; }
			public bool Touched { get; set; }

			public static Table1 Read(BinaryReader reader)
			{
				return new Table1()
				{
					Zero00 = reader.ReadInt32(),
					Zero04 = reader.ReadInt32(),
					Table5 = reader.ReadInt32(),
					ElementsCount = reader.ReadInt32(),
					Table4 = reader.ReadInt32(),
					Unk14 = reader.ReadInt32(),
					Name = reader.ReadInt32(),
					Table6 = reader.ReadInt32(),
				};
			}
		}

		public class Table2
		{
			public int Zero00 { get; set; }
			public int Zero04 { get; set; }
			public int Name { get; set; }
			public int Table6 { get; set; }

			public string Str { get; set; }
			public bool Touched { get; set; }

			public static Table2 Read(BinaryReader reader)
			{
				return new Table2()
				{
					Zero00 = reader.ReadInt32(),
					Zero04 = reader.ReadInt32(),
					Name = reader.ReadInt32(),
					Table6 = reader.ReadInt32(),
				};
			}
		}

		public class TableValue
		{
			public ValueType Type { get; set; }
			public int Name { get; set; }
			public int Value1 { get; set; }
			public int Value2 { get; set; }
			public int Value3 { get; set; }
			public int Value4 { get; set; }

			public string Str { get; set; }
			public bool Touched { get; set; }

			public override string ToString()
			{
				switch (Type)
				{
					case ValueType.Unknown:
						return Value1 == 0 && Value2 == 0 && Value3 == 0 && Value4 == 0 ? "" : "???";
					case ValueType.Bool:
						return (Value1 != 0).ToString();
					case ValueType.Signed:
						return Value1.ToString();
					case ValueType.Unsigned:
						return Value1.ToString();
					case ValueType.Vector4:
						return $"{ToFloat(Value1)},{ToFloat(Value2)},{ToFloat(Value3)},{ToFloat(Value4)}";
					default:
						return $"{Type}?? {Value1.ToString("X08")} {Value2.ToString("X08")} {Value3.ToString("X08")} {Value4.ToString("X08")}";
				}
			}

			public static TableValue Read(BinaryReader reader)
			{
				return new TableValue()
				{
					Type = (ValueType)reader.ReadInt32(),
					Name = reader.ReadInt32(),
					Value1 = reader.ReadInt32(),
					Value2 = reader.ReadInt32(),
					Value3 = reader.ReadInt32(),
					Value4 = reader.ReadInt32(),
				};
			}
		}

		private Table1[] t1;
		private Table2[] t2;
		private TableValue[] t6;
		private int[] t4;
		private int[] t5;
		private bool[] t5t;

		public Xmb(BinaryReader reader)
		{
			var header = Header.Read(reader);
			Debug.Assert(header.Reserved3c == 0);
			Debug.Assert(header.Reserved40 == 0);
			Debug.Assert(header.Reserved44 == 0);
			Debug.Assert(header.Reserved48 == 0);
			Debug.Assert(reader.BaseStream.Position == header.Table1.Offset);

			reader.BaseStream.Position = header.Table1.Offset;
			t1 = new Table1[header.Table1.Count];
			for (int i = 0; i < t1.Length; i++)
			{
				t1[i] = Table1.Read(reader);
			}

			reader.BaseStream.Position = header.Table2.Offset;
			t2 = new Table2[header.Table2.Count];
			for (int i = 0; i < t2.Length; i++)
			{
				t2[i] = Table2.Read(reader);
			}

			reader.BaseStream.Position = header.Table6.Offset;
			t6 = new TableValue[header.Table6.Count];
			for (int i = 0; i < t6.Length; i++)
			{
				t6[i] = TableValue.Read(reader);
			}

			reader.BaseStream.Position = header.Table4.Offset;
			t4 = new int[header.Table4.Count];
			for (int i = 0; i < t4.Length; i++)
			{
				t4[i] = reader.ReadInt32();
			}

			reader.BaseStream.Position = header.Table5.Offset;
			t5 = new int[header.Table5.Count];
			for (int i = 0; i < t5.Length; i++)
			{
				t5[i] = reader.ReadInt32();
			}

			if (header.Unk38 < header.Table1.Count)
			{
				int off = header.Table1.Offset + 0x20 * header.Unk38;
			}
			else
			{

			}

			int t1T3 = 0, t1T4 = 0, t1T5 = 0, t1U1 = 0, t1U2 = 0;
			for (int i = 0; i < t1.Length; i++)
			{
				t1[i].Str = ReadString(reader, header.TableNames.Offset + t1[i].Name);
				t1T3 = Math.Max(t1T3, t1[i].Table6);
				t1T4 = Math.Max(t1T4, t1[i].Table4);
				t1T5 = Math.Max(t1T5, t1[i].Table5);
				t1U1 = Math.Max(t1U1, t1[i].ElementsCount);
				t1U2 = Math.Max(t1U2, t1[i].Unk14);
				Debug.Assert(t1[i].Zero00 == 0);
				Debug.Assert(t1[i].Zero04 == 0);
				Debug.Assert(t1[i].Table4 < t4.Length);
				Debug.Assert(t1[i].Table5 < t5.Length);
			}

			int t2T3 = 0;
			for (int i = 0; i < t2.Length; i++)
			{
				t2[i].Str = ReadString(reader, header.TableNames.Offset + t2[i].Name);
				t2T3 = Math.Max(t2T3, t2[i].Table6);
				Debug.Assert(t2[i].Zero00 == 0);
				Debug.Assert(t2[i].Zero04 == 0);
				Debug.Assert(t2[i].Table6 < t6.Length);
			}

			for (int i = 0; i < t6.Length; i++)
			{
				t6[i].Str = ReadString(reader, header.TableNames.Offset + t6[i].Name);
			}

			int t4T1 = 0;
			for (int i = 0; i < t4.Length; i++)
			{
				t4T1 = Math.Max(t4T1, t4[i]);
				Debug.Assert(t4[i] < t1.Length);
			}

			int t5T2 = 0;
			t5t = new bool[t5.Length];
			for (int i = 0; i < t5.Length; i++)
			{
				t5T2 = Math.Max(t5T2, t5[i]);
				Debug.Assert(t5[i] < t2.Length);
			}
		}

		public IEnumerable<Variable> GetVariables()
		{
			for (int i = 0; i < t1.Length; i++)
			{
				var _1 = t1[i];
				_1.Touched = true;
				t6[_1.Table6].Touched = true;
				var v = new Variable()
				{
					Name = _1.Str,
					Value = t6[_1.Table6].Str,
					Unk2 = _1.Unk14,
					Sub = new List<Sub>(_1.ElementsCount)
				};

				t5t[_1.Table5] = true;
				for (int j = 0; j < _1.ElementsCount; j++)
				{
					var i2 = t2[t5[_1.Table5] + j];
					i2.Touched = true;
					t6[i2.Table6].Touched = true;
					v.Sub.Add(new Sub()
					{
						DebugIndex = t5[_1.Table5] + j,
						Decl = i2.Str,
						Type = t6[i2.Table6].Str,
					});
				}
				
				yield return v;
			}
		}

		public void EvaluateUnknownTypes()
		{
			for (int i = 0; i < t6.Length; i++)
			{
				switch (t6[i].Type)
				{
					case ValueType.Unknown:
					case ValueType.Bool:
					case ValueType.Signed:
					case ValueType.Unsigned:
					case ValueType.Float:
					case ValueType.Vector4:
						break;
					default:
						Console.WriteLine($"Unknown type {t6[i].Type}: {t6[i].Str}");
						break;
				}
			}
		}

		public void Touch()
		{
			foreach (var i1 in t1)
			{
				t6[i1.Table6].Touched = true;
				t5t[i1.Table5] = true;
				var i5 = t5[i1.Table5];
				t2[i5].Touched = true;
				t6[t2[i5].Table6].Touched = true;
			}
			//foreach (var i5 in t5)
			//{
			//	t2[i5].Touched = true;
			//	t3[t2[i5].Table3].Touched = true;
			//}
		}

		public void EvaluateUntouched()
		{
			var orphans = t5t.Select((x, i) => new { x, i }).Where(x => !x.x).Select(x => x.i);
			Console.WriteLine($"T5 orphans: {t5t.Count(x => !x)}");
			Console.WriteLine($"{string.Join(",", orphans)}");

			Console.WriteLine($"T2 orphans: {t2.Count(x => !x.Touched)}");
			for (int i = 0; i < t2.Length; i++)
			{
				var i2 = t2[i];
				if (!i2.Touched)
				{
					t6[i2.Table6].Touched = true;
					Console.WriteLine($"{i}\t{i2.Str} {t6[i2.Table6].Str}");
				}
			}
			Debug.Assert(t6.Count(x => !x.Touched) == 0);
		}

		private string ReadString(BinaryReader reader, int offset)
		{
			reader.BaseStream.Position = offset;
			return ReadString(reader);
		}

		private string ReadString(BinaryReader reader)
		{
			var buffer = new byte[0x100];
			var index = 0;
			byte c;

			while ((c = reader.ReadByte()) != 0)
			{
				buffer[index++] = c;
			}

			return Encoding.UTF8.GetString(buffer, 0, index);
		}

		private static float ToFloat(int n)
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(n), 0);
		}
	}
}
