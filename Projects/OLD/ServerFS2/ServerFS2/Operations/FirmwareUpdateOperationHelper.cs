using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FiresecAPI.Models;
using Infrastructure.Common;
using Ionic.Zip;

namespace ServerFS2
{
	public static class FirmwareUpdateOperationHelper
	{
		public static string Verify(Device device, bool isUSB, string fileName)
		{
			var hexInfo = GetHexInfo(fileName);
			return "В приборе записана более поздняя версия прошиви. Продолжить?";
		}

		public static void Update(Device device, bool isUSB, string fileName)
		{
			var hexInfo = GetHexInfo(fileName);
		}

		public static HexInfo GetHexInfo(string fileName, string hexFileName = "firmware.hex")
		{
			var hexInfo = new HexInfo();
			var zipFile = new ZipFile(fileName, Encoding.GetEncoding("cp866"))
			{
				Password = "adm"
			};
			var tempFolder = AppDataFolderHelper.GetTempFolder();
			zipFile.ExtractAll(tempFolder);

			hexFileName = Path.Combine(tempFolder, hexFileName);
			var strings = File.ReadAllLines(hexFileName).ToList();
			hexInfo.Offset = Convert.ToInt32(strings[strings.Count - 2].Substring(9, 8), 16);
			//strings.RemoveAt(0);
			strings.RemoveRange(strings.Count - 2, 2);
			var pattern = @"[:][0][2]+.*";
			var regex = new Regex(pattern);
			var mathes = new List<string>();
			foreach (var str in strings)
			{
				var match = regex.Match(str);
				if (match.Value != "")
				{
					mathes.Add(match.Value);
					continue;
				}
				var count = Convert.ToInt32(str.Substring(1, 2), 16);
				for (var i = 9; i < count * 2 + 9; i += 2)
				{
					hexInfo.Bytes.Add(Convert.ToByte(str.Substring(i, 2), 16));
				}
			}

			if (Directory.Exists(tempFolder))
				Directory.Delete(tempFolder, true);

			return hexInfo;
		}
	}

	public class HexInfo
	{
		public HexInfo()
		{
			Bytes = new List<byte>();
		}

		public int Offset { get; set; }
		public List<byte> Bytes { get; set; }
	}
}