using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Infrastructure.Common;
using Ionic.Zip;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static class SetConfigurationOperationHelper
	{
		static List<byte> _crc;

		public static List<byte> GetFirmwhareBytes(Device device)
		{
			var firmwhareFileName = AppDataFolderHelper.GetServerAppDataPath("frm.zip");
			var folderName = AppDataFolderHelper.GetFolder("Server");
			var configFileName = Path.Combine(folderName, "frm.zip");
			var zipFile = ZipFile.Read(configFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(configFileName);
			var unzipFolderPath = Path.Combine(fileInfo.Directory.FullName, "Unzip");
			if (Directory.Exists(unzipFolderPath))
				Directory.Delete(unzipFolderPath, true);
			zipFile.ExtractAll(unzipFolderPath);

			var fileName = device.Driver.ShortName + ".hex";
			var configurationFileName = Path.Combine(unzipFolderPath, fileName);
			var strings = File.ReadAllLines(configurationFileName).ToList();
			strings.RemoveAt(0);
			strings.RemoveAt(strings.Count - 1);
			var bytes = new List<byte>();
			foreach (var str in strings)
			{
				var count = Convert.ToInt32(str.Substring(1, 2), 16);
				for (var i = 9; i < count * 2 + 9; i = i + 2)
				{
					bytes.Add(Convert.ToByte(str.Substring(i, 2), 16));
				}
			}
			return bytes;
		}

		static void SetFlashConfig(Device device, List<byte> deviceFlash)
		{
			deviceFlash.RemoveRange(0, 0x100);
			_crc = new List<byte>() { deviceFlash[0], deviceFlash[1] };
			deviceFlash[0] = 0;
			deviceFlash[1] = 0;
			USBManager.SendCodeToPanel(device, 0x02, 0x52, BitConverter.GetBytes(0x100).Reverse(), 0x96, deviceFlash);
			USBManager.SendCodeToPanel(device, 0x02, 0x52, BitConverter.GetBytes(0x100).Reverse(), 0x01, _crc[0], _crc[1]);
		}

		private static void SetRomConfig(Device device, List<byte> deviceRom, int begin)
		{
			List<byte> bytes;
			for (int i = 0; i < deviceRom.Count; i = i + 0x100)
			{
				USBManager.SendCodeToPanel(device, 0x3E, BitConverter.GetBytes(begin + i).Reverse(), deviceRom.GetRange(i, Math.Min(deviceRom.Count - i, 0x100)));
			}
		}

		public static void SetDeviceConfig(Device device, List<byte> Rom, List<byte> Flash)
		{
			var RomDBFirstIndex = ServerHelper.GetRomFirstIndex(device);
			var FlashDBLastIndex = ServerHelper.GetFlashLastIndex(device);
			Rom = ServerHelper.GetRomDBBytes(device);
			Flash = ServerHelper.GetFlashDBBytes(device);
			//SendCode(CreateBytesArray(0x01, 0x02, 0x34, 0x01)); // Запись в MDS
			//SendCode(CreateBytesArray(0x01, 0x02, 0x37)); // Информация о прошивке
			BlockBD(device);
			SetFlashConfig(device, Flash);
			var delay = (int)Math.Pow(2, USBManager.SendCodeToPanel(device, 0x39, 0x01)[0]);
			Thread.Sleep(delay);
			ConfirmationLongTermOperation(device);
			var firmwhareBytes = GetFirmwhareBytes(device);
			SetRomConfig(device, firmwhareBytes, 0x40000000);
			delay = (int)Math.Pow(2, USBManager.SendCodeToPanel(device, 0x39, 0x04)[0]);
			ConfirmationLongTermOperation(device);
			ClearSector(device);
			Thread.Sleep(delay);
			SetRomConfig(device, Rom, RomDBFirstIndex);
			StopUpdating(device);
			//ConfirmationLongTermOperation(device);
		}

		// Окончание записи памяти - сброс
		private static void StopUpdating(Device device)
		{
			USBManager.SendCodeToPanel(device, 0x3A);
		}

		// Установка блокировки БД
		private static void BlockBD(Device device)
		{
			USBManager.SendCodeToPanel(device, 0x02, 0x55, 0x01);
		}

		// Очистка сектора памяти bSectorStart, bSectorEnd
		private static void ClearSector(Device device)
		{
			USBManager.SendCodeToPanel(device, 0x3B, 0x03, 0x04);
		}

		// Подтверждение / завершение долговременной операции
		private static void ConfirmationLongTermOperation(Device device)
		{
			USBManager.SendCodeToPanel(device, 0x3C);
		}
	}
}