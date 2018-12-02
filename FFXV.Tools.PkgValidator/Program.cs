using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FFXV.Services;

namespace FFXV.Tools.PkgValidator
{
	public class Program
	{
		static void Main(string[] args)
		{
			var asd = new List<string>();
			var filesToProcess = GetFilesList(asd, ".", true, x =>
			{
				switch (Path.GetExtension(x).ToLower())
				{
					case ".xml":
						return true;
					default:
						return false;
				}
			});

			var typesFound = new Dictionary<string, bool>();

			for (var i = 0; i < filesToProcess.Count; ++i)
			{
				var fileName = filesToProcess[i];
				var document = GetDocument(fileName);

				var percentage = (i + 1) * 100 / filesToProcess.Count;
				Console.Write($"\rProcessing... {percentage:D03}%");

				typesFound = PackageService.GetSupportedTypes(typesFound, document.Root);
			}

			const string logFileName = "pkgreport.log";
			Console.WriteLine($"Writing report to {logFileName}...");
			WriteReport(logFileName, typesFound);
		}

		private static XDocument GetDocument(string fileName)
		{
			using (var stream = File.Open(fileName, FileMode.Open))
			{
				switch (Path.GetExtension(fileName))
				{
					case ".xml":
						return XDocument.Load(stream);
					case ".xmb":
						return new Xmb(new BinaryReader(stream)).Document;
					case ".xmb2":
						return new Xmb2(new BinaryReader(stream)).Document;
					default:
						return null;
				}
			}
		}

		private static void WriteReport(string fileName, Dictionary<string, bool> typesFound)
		{
			if (typesFound.Count == 0)
				return;

			using (var stream = new StreamWriter(fileName))
			{
				var elementsCount = typesFound.Count;
				var found = 0;

				var reportData = typesFound
					.OrderBy(x => x.Key)
					.Select(x => new
					{
						Name = x.Key,
						Found = x.Value
					});

				foreach (var item in reportData)
				{
					char chFound;
					if (item.Found)
					{
						chFound = 'x';
						++found;
					}
					else
					{
						chFound = ' ';
					}

					stream.WriteLine($"[{chFound}] {item.Name}");
				}

				stream.WriteLine($"\nFound {found}/{elementsCount} ({found * 100 / elementsCount}%)");
			}
		}

		private static List<string> GetFilesList(List<string> list, string path, bool includeSubDirectories, Func<string, bool> expression)
		{
			foreach (var file in Directory.GetFiles(path))
			{
				if (expression(file))
				{
					list.Add(file);
				}
			}

			if (includeSubDirectories)
			{
				foreach (var dir in Directory.GetDirectories(path))
				{
					GetFilesList(list, dir, includeSubDirectories, expression);
				}
			}

			return list;
		}
	}
}
