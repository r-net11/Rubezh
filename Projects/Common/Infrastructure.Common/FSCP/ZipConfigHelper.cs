using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace Infrastructure.Common
{
	public static class ZipConfigHelper
	{
		public static void IntoZip(string fileName, MemoryStream stream)
		{
			var path = ".\\Configuration\\";
			var zipName = "config.fscp";
			var filePath = path + fileName;
			var zipPath = path + zipName;
			var zip = new ZipFile(zipPath);
			if (zip.Entries.FirstOrDefault(x => x.FileName == fileName) != null)
				zip.RemoveEntry(fileName);
			stream.Position = 0;
			zip.AddEntry(fileName, stream);
			zip.Save(zipPath);
		}

		public static MemoryStream FromZip(string fileName)
		{
			if (!File.Exists("Configuration//config.fscp"))
				return null;
			var path = ".\\Configuration\\";
			var zipName = "config.fscp";
			var zipPath = path + zipName;

			var stream = new MemoryStream();
			using (var unzip = ZipFile.Read(zipPath, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") }))
			{
				var entry = unzip[fileName];
				if (entry == null)
					return null;
				entry.Extract(stream);
				stream.Position = 0;
				return stream;
			}
		}
	}
}