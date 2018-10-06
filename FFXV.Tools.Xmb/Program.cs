using FFXV.Services;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace FFXV.Tools.XmbTool
{
	[Command(Description = "Manage Luminous Engine's XMB files, which have EBEX as extension")]
	class Program
	{
		private static readonly string ExtXml = ".xml";
		private static readonly string ExtExml = ".exml";

		private delegate void Operation(string input, string output);

		public static int Main(string[] args)
		{
			CommandLineApplication.Execute<Program>(args);
			return 0;
		}


		[Option("-x|--export", "Specify if the input is the one that has to be imported", CommandOptionType.NoValue, LongName = "--export")]
		[Required]
		public bool IsExport { get; }

		[Option("-i|--input", "Input file or folder", CommandOptionType.SingleValue, LongName = "input")]
		[Required]
		public string Input { get; }

		[Option("-o|--output", "Output file or folder", CommandOptionType.SingleValue, LongName = "output")]
		[Required]
		public string Output { get; }

		[Option("-v|--verbose", "Print all the information", CommandOptionType.NoValue, LongName = "verbose")]
		public bool IsVerbose { get; }

		[Option("-d|--directory", "Process a directory instead of a file", CommandOptionType.NoValue, LongName = "directory")]
		public bool IsDirectory { get; }

		[Option("-r|--recursive", "Recursively process all the sub-directories", CommandOptionType.NoValue, LongName = "recursive")]
		public bool IsRecursive { get; }

		private void OnExecute()
		{
			if (IsDirectory)
			{
				Export(Input);
			}
			else
			{
				Export(Input, Output);
			}
		}

		private void Export(string path)
		{
			foreach (var file in Directory.GetFiles(path))
			{
				if (file.EndsWith(ExtExml))
				{
					Do(Export, file, file.Replace(ExtExml, ExtXml));
				}
			}

			if (IsRecursive)
			{
				foreach (var dir in Directory.GetDirectories(path))
				{
					Export(dir);
				}
			}
		}

		private void Export(string input, string output)
		{
			if (IsVerbose)
				Console.WriteLine(input);

			using (var inStream = File.Open(input, FileMode.Open))
			{
				using (var outStream = File.Open(output, FileMode.Create))
				{
					Xmb.Open(inStream).Save(outStream);
				}
			}
		}

		private void Import(string path)
		{
			foreach (var file in Directory.GetFiles(path))
			{
				if (file.EndsWith(ExtXml))
				{
					Do(Export, file, file.Replace(ExtXml, ExtExml));
				}
			}

			if (IsRecursive)
			{
				foreach (var dir in Directory.GetDirectories(path))
				{
					Import(dir);
				}
			}
		}

		private void Import(string input, string output)
		{
			if (IsVerbose)
				Console.WriteLine(input);

			using (var inStream = File.Open(input, FileMode.Open))
			{
				using (var outStream = File.Open(output, FileMode.Create))
				{
					Xmb.Save(inStream, XDocument.Load(inStream).Root);
				}
			}
		}

		private void Do(Operation operation, string input, string output)
		{
			if (IsVerbose)
				Console.WriteLine(input);

			try
			{
				operation(input, output);
			}
			catch (Exception ex)
			{
				var prevColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"{input}: {ex.Message}");
				Console.ForegroundColor = prevColor;
			}
		}
	}
}
