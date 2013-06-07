using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Infrastructure.Common;
using Ionic.Zip;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static partial class ServerHelper
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

		static void SetRomConfig(Device device, List<byte> deviceRom)
		{
			deviceRom.RemoveRange(0, 0x100);
			_crc = new List<byte>() { deviceRom[0], deviceRom[1] };
			deviceRom[0] = 0;
			deviceRom[1] = 0;
			var bytes = CreateBytesArray(0x02, 0x52, BitConverter.GetBytes(0x100).Reverse(), 0x96, deviceRom);
			SendCodeToPanel(bytes, device);
			bytes = CreateBytesArray(0x02, 0x52, BitConverter.GetBytes(0x100).Reverse(), 0x01, _crc[0], _crc[1]);
			SendCodeToPanel(bytes, device);
		}

		private static void SetFlashConfig(Device device, List<byte> deviceFlash, int begin)
		{
			List<byte> bytes;
			for (int i = 0; i < deviceFlash.Count ; i = i + 0x100)
			{
				bytes = CreateBytesArray(0x3E, BitConverter.GetBytes(begin + i).Reverse(), deviceFlash.GetRange(i, Math.Min(deviceFlash.Count - i, 0x100)));
				SendCodeToPanel(bytes, device);
			}
			//SendCode(CreateBytesArray(0x1C, 0x82, 0xA9, 0x7A, 0x04, 0x01, 0x3D, 0x1E, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x74, 0x00, 0x00, 0x01, 0x00, 0x01, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x77, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
			//, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x3B, 0x01, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00
			//, 0x00, 0x00, 0x01, 0x02, 0x00, 0x01, 0x5B, 0x00, 0x01, 0x67));
		}

		public static void SetDeviceConfig(Device device, List<byte> Rom, List<byte> Flash)
		{
			RomDBFirstIndex = GetRomFirstIndex(device);
			FlashDBLastIndex = GetFlashLastIndex(device);
			
			SendCode(CreateBytesArray(0x01, 0x02, 0x34, 0x01));
			SendCode(CreateBytesArray(0x01, 0x02, 0x37));
			SendCode(CreateBytesArray(0x04, 0x01, 0x02, 0x55, 0x01));
			
			SetRomConfig(device, Rom);
			var delay = Math.Pow(2, SendCodeToPanel(CreateBytesArray(0x39, 0x01), device)[0]);
			Thread.Sleep(100);
			ConfirmationLongTermOperation(device);
			var firmwhareBytes = GetFirmwhareBytes(device);
			SetFlashConfig(device, firmwhareBytes, 0x40000000);
			ClearSector(device);
			ConfirmationLongTermOperation(device);
			delay = Math.Pow(2, SendCodeToPanel(CreateBytesArray(0x39, 0x01), device)[0]);
			Thread.Sleep(100);
			SetFlashConfig(device, Flash, RomDBFirstIndex);
			SendCode(CreateBytesArray(0x04, 0x01, 0x3A));
			SendCode(CreateBytesArray(0x04, 0x01, 0x3C));
		}

		static List<byte> CreateInputBytes(List<byte> messageBytes)
		{
			var bytes = new List<byte>();
			var previousByte = new byte();
			messageBytes.RemoveRange(0, messageBytes.IndexOf(0x7E) + 1);
			messageBytes.RemoveRange(messageBytes.IndexOf(0x3E), messageBytes.Count - messageBytes.IndexOf(0x3E));
			foreach (var b in messageBytes)
			{
				if ((b == 0x7D) || (b == 0x3D))
				{ previousByte = b; continue; }
				if (previousByte == 0x7D)
				{
					previousByte = new byte();
					if (b == 0x5E)
					{ bytes.Add(0x7E); continue; }
					if (b == 0x5D)
					{ bytes.Add(0x7D); continue; }
				}
				if (previousByte == 0x3D)
				{
					previousByte = new byte();
					if (b == 0x1E)
					{ bytes.Add(0x3E); continue; }
					if (b == 0x1D)
					{ bytes.Add(0x3D); continue; }
				}
				bytes.Add(b);
			}
			return bytes;
		}

		// Очистка сектора памяти bSectorStart, bSectorEnd
		private static void ClearSector(Device device)
		{
			var bytes = CreateBytesArray(0x3B, 0x03, 0x04);
			SendCodeToPanel(bytes, device);
		}

		// Подтверждение / завершение долговременной операции
		private static void ConfirmationLongTermOperation(Device device)
		{
			var bytes = CreateBytesArray(0x3C);
			SendCodeToPanel(bytes, device);
		}
	}
}