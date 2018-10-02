using FFXV.Models;
using Google.Protobuf;
using System;
using System.IO;

namespace FFXV
{
	public class Program
	{
		static void Main(string[] args)
		{
			TestXmb();
		}

		static void TestXmb()
		{
			const string FileName = @"XMB1\ctrl_cmn_text_icon.exml";
			//const string FileName = @"XMB1\ctrl_exp_normal.exml";
			//const string FileName = @"XMB1\initialize1.exml";
			//const string FileName = @"XMB1\ctrl_game_over.exml";
			//const string FileName = @"XMB1\layout_title_epd.exml";
			//const string FileName = @"XMB1\commonai.exml";
			//const string FileName = @"XMB1\debug_wm.exml";

			using (var stream = File.OpenRead(FileName))
			{
				var o = new FFXV.Services.Xmb(new BinaryReader(stream));
				foreach (var item in o.GetVariables())
				{
					//Console.WriteLine($"{item.Name} {item.Value} ({item.Unk2})");
					Console.WriteLine($"{item.Name} {item.Value}");
					foreach (var sub in item.Sub)
					{
						//Console.WriteLine($"\t{sub.DebugIndex}\t{sub.Decl} {sub.Type}");
						Console.WriteLine($"\t{sub.Decl} {sub.Type}");
					}
				}
				o.EvaluateUntouched();
				o.EvaluateUnknownTypes();
			}

			Console.ReadLine();
		}

		static void TestXmb2()
		{
			const string FileName = @"XMB2\initialize1.exml";

			using (var stream = File.OpenRead(FileName))
			{
				var o = new FFXV.Services.Xmb2(new BinaryReader(stream));
			}
		}
	}
}
