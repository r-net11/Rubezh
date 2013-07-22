using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Infrastructure.Common;
using Ionic.Zip;

namespace HexManager
{
	public static class HexZipHelper
	{
		public static void Zip(string folderName)
		{
			var tempFolderName = AppDataFolderHelper.GetTempFolder();
			Directory.CreateDirectory(tempFolderName);

			foreach (var fileName in Directory.GetFiles(folderName))
			{
				var fileInfo = new FileInfo(fileName);
				File.Copy(fileName, Path.Combine(tempFolderName, fileInfo.Name));
			}

			var hexFileName = Path.Combine(tempFolderName, "Firmware.hex");
			var encryptedHexFileName = Path.Combine(tempFolderName, "EncryptedFirmware.hex");
			using (var zipFile = new ZipFile(hexFileName))
			{
				zipFile.AddDirectory(folderName);
				zipFile.Save(hexFileName);
			}

			EncryptHelper.EncryptFile(hexFileName, encryptedHexFileName, "adm");
			File.Delete(hexFileName);
			Directory.Delete(tempFolderName);
			File.Copy(encryptedHexFileName, "C:/Firmware.hex");
		}

		public static void Unzip(string fileName)
		{
			var tempFolderName = AppDataFolderHelper.GetTempFolder();
			Directory.CreateDirectory(tempFolderName);

			var hexFileName = Path.Combine(tempFolderName, "Firmware.hex");
			var encryptedHexFileName = Path.Combine(tempFolderName, "EncryptedFirmware.hex");
			File.Copy(fileName, encryptedHexFileName);

			EncryptHelper.DecryptFile(encryptedHexFileName, hexFileName, "adm");

			var unzipFolderPath = Path.Combine(tempFolderName, "Unzip");
			using (var zipFile = ZipFile.Read(hexFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") }))
			{
				var fileInfo = new FileInfo(hexFileName);
				zipFile.ExtractAll(unzipFolderPath);
			}
		}
	}
}