using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using FiresecAPI.Models;
using Infrastructure.Common;
using Ionic.Zip;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static class SetConfigurationOperationHelper
	{
		static List<byte> _crc;

		public static bool UpdateFoolFlash(Device device)
		{
			// 01 01, 01 03, 37 02, 37 03, 37 01
			BeginUpdateFirmWare(device);
			ConfirmLongTermOperation(device);
			// 3D
			var firmwareFileName = Path.Combine(AppDataFolderHelper.GetFolder("Server"), "2OP\\frm.fscf");
			var hexInfo = FirmwareUpdateOperationHelper.GetHexInfo(firmwareFileName, device.Driver.ShortName + ".hex");
			var r2amInfo = FirmwareUpdateOperationHelper.GetHexInfo(firmwareFileName, "R2OP_loader_v1.03_CRP.hex");
			var rubezhOPSInfo = FirmwareUpdateOperationHelper.GetHexInfo(firmwareFileName, "Rubezh_2OP.hex");
			WriteRomConfiguration(device, hexInfo.Bytes, hexInfo.Offset);
			BeginUpdateRom(device);
			ConfirmLongTermOperation(device);
			// 3D, 01 01

			WriteRomConfiguration(device, r2amInfo.Bytes, r2amInfo.Offset); // Запись прошивки с 00 00 по 00 2F
			ConfirmLongTermOperation(device);
			// 3D
			ClearAvrSector(device);
			ConfirmLongTermOperation(device);
			// 3D
			WriteRomConfiguration(device, rubezhOPSInfo.Bytes, 0x5000); // Запись прошивки с 50 00 по 07 CF
			USBManager.Send(device, 0x02, 0x12, 0x02, 0x30);// 02 12 version(For Example 02 30)
			// 01 12
			StopUpdating(device);
			USBManager.Send(device.ParentUSB, 0x01, 0x36);
			ConfirmLongTermOperation(device);
			/*
			var foolFlashFileName = Path.Combine(AppDataFolderHelper.GetFolder("Server"), "fullflash2am.zip");
			var foolFlashHexInfo = FirmwareUpdateOperationHelper.GetHexInfo(foolFlashFileName, "fullflash2am.hex");
			foolFlashHexInfo.Offset = 0;

			WriteFoolFlashConfiguration(device, @"C:\test\soft.txt");

			ConfirmLongTermOperation(device);
			ClearSector(device);
			ConfirmLongTermOperation(device);

			var avrHexInfo = FirmwareUpdateOperationHelper.GetHexInfo(@"C:\test\", "Rubezh_OPS.hex");
			WriteRomConfiguration(device, avrHexInfo.Bytes, avrHexInfo.Offset);

			StopUpdating(device);
			ConfirmLongTermOperation(device);
			ServerHelper.SynchronizeTime(device);
			*/

			return true;
		}

		public static bool WriteDeviceConfiguration(Device device, List<byte> Flash, List<byte> Rom)
		{
			UpdateFoolFlash(device);
			//var panelDatabaseReader = new ReadPanelDatabaseOperationHelper(device, false);
			//var romDBFirstIndex = panelDatabaseReader.GetRomFirstIndex(device);
			//BlockBD(device);
			//WriteFlashConfiguration(device, Flash);
			//Thread.Sleep(BeginUpdateFirmWare(device));
			//ConfirmLongTermOperation(device);
			//var firmwareFileName = Path.Combine(AppDataFolderHelper.GetFolder("Server"), "frm.fscf");
			//var hexInfo = FirmwareUpdateOperationHelper.GetHexInfo(firmwareFileName, device.Driver.ShortName + ".hex");
			//WriteRomConfiguration(device, hexInfo.Bytes, hexInfo.Offset);
			//Thread.Sleep(BeginUpdateRom(device));
			//ConfirmLongTermOperation(device);
			//ClearSector(device);
			//ConfirmLongTermOperation(device);
			//WriteRomConfiguration(device, Rom, romDBFirstIndex);
			//StopUpdating(device);
			//ConfirmLongTermOperation(device);
			//ServerHelper.SynchronizeTime(device);
			return true;
		}

		static void BeginUpdateRom(Device device)
		{
			var delayBytes = USBManager.Send(device, 0x39, 0x04);
			int delay = 0;
			if (delayBytes.Bytes.Count > 1)
				delay = (int)Math.Pow(2, delayBytes.Bytes[1]);
			Thread.Sleep(delay);
		}

		static void BeginUpdateFirmWare(Device device)
		{
			var delayBytes = USBManager.Send(device, 0x39, 0x01);
			int delay = 0;
			if (delayBytes.Bytes.Count > 1)
				delay = (int)Math.Pow(2, delayBytes.Bytes[1]);
			Thread.Sleep(delay);
		}

		static bool GetStatusOS(Device device)
		{
			if (USBManager.Send(device, 0x01, 0x01).Bytes[1] == 0) // Если младший байт = 0, то режим пользовательский
				return true;
			return false;
		}

		static void WriteFlashConfiguration(Device device, List<byte> deviceFlash)
		{
			deviceFlash.RemoveRange(0, 0x100);
			_crc = new List<byte> { deviceFlash[0], deviceFlash[1] };
			deviceFlash[0] = 0;
			deviceFlash[1] = 0;
			for (int i = 0; i < deviceFlash.Count - 1; i = i + 0x100)
				USBManager.Send(device, 0x02, 0x52, BitConverter.GetBytes(0x100 + i).Reverse(), Math.Min(deviceFlash.Count - i - 1, 0xFF), deviceFlash.GetRange(i, Math.Min(deviceFlash.Count - i, 0x100)));
			USBManager.Send(device, 0x02, 0x52, BitConverter.GetBytes(0x100).Reverse(), 0x01, _crc[0], _crc[1]);
		}

		static void WriteRomConfiguration(Device device, List<byte> deviceRom, int begin)
		{
			for (int i = 0; i < deviceRom.Count; i = i + 0x100)
			{
				USBManager.Send(device, 0x3E, BitConverter.GetBytes(begin + i).Reverse(), deviceRom.GetRange(i, Math.Min(deviceRom.Count - i, 0x100)));
			}
		}

		static void WriteFoolFlashConfiguration(Device device, string fileName)
		{
			var bytesArray = new List<byte>();
			var strings = File.ReadAllLines(fileName).ToList();
			foreach (var str in strings)
			{
				for (var i = 0; i < str.Length ; i += 3)
				{
					bytesArray.Add(Convert.ToByte(str.Substring(i, 2), 16));
				}
			}

			for (int i = 0; i < bytesArray.Count; i = i + 0x104)
			{
				var offset = BytesHelper.ExtractInt(bytesArray, i);
				USBManager.Send(device, 0x3E, BitConverter.GetBytes(offset).Reverse(), bytesArray.GetRange(i, Math.Min(bytesArray.Count - i, 0x100)));
			}
		}

		// Окончание записи памяти - сброс
		private static void StopUpdating(Device device)
		{
			USBManager.Send(device, 0x3A);
		}

		// Установка блокировки БД
		private static void BlockBD(Device device)
		{
			USBManager.Send(device, 0x02, 0x55, 0x01);
			var status = GetAddressList(device)[1];
			while (status != 0x00)
				status = GetAddressList(device)[1];
		}

		private static List<byte> GetAddressList(Device device)
		{
			var result = USBManager.Send(device, 0x01, 0x14).Bytes;
			return result;
		}

		// Очистка сектора памяти bSectorStart, bSectorEnd
		private static void ClearSector(Device device)
		{
			USBManager.Send(device, 0x3B, 0x03, 0x04);
		}

		private static void ClearAvrSector(Device device)
		{
			USBManager.Send(device, 0x3B, 0x05, 0x1A);
		}

		// Подтверждение / завершение долговременной операции
		private static void ConfirmLongTermOperation(Device device)
		{
			var functionCode = USBManager.Send(device, 0x3C).FunctionCode;
			if (functionCode != 0x7c)
				ConfirmLongTermOperation(device);
		}
	}
}