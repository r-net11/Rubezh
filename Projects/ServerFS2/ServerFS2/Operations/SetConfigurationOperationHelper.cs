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

		public static void WriteDeviceConfiguration(Device device, List<byte> Flash, List<byte> Rom)
		{
			var panelDatabaseReader = new ReadPanelDatabaseOperationHelper(device, false);
			var romDBFirstIndex = panelDatabaseReader.GetRomFirstIndex(device);
			BlockBD(device);
			WriteFlashConfiguration(device, Flash);
			Thread.Sleep(BeginUpdateFirmWare(device));
			ConfirmLongTermOperation(device);
			var firmwareFileName = Path.Combine(AppDataFolderHelper.GetFolder("Server"), "frm.fscf");
			var hexInfo = FirmwareUpdateOperationHelper.GetHexInfo(firmwareFileName, device.Driver.ShortName + ".hex");
			WriteRomConfiguration(device, hexInfo.Bytes, hexInfo.Offset);
			Thread.Sleep(BeginUpdateRom(device));
			ConfirmLongTermOperation(device);
			ClearSector(device);
			ConfirmLongTermOperation(device);
			WriteRomConfiguration(device, Rom, romDBFirstIndex);
			StopUpdating(device);
			ConfirmLongTermOperation(device);
			ServerHelper.SynchronizeTime(device);
		}

		static int BeginUpdateRom(Device device)
		{
			var delayBytes = USBManager.Send(device, 0x39, 0x04);
			int delay = 0;
			if (delayBytes.Bytes.Count > 1)
				delay = (int)Math.Pow(2, delayBytes.Bytes[1]);
			return delay;
		}

		static int BeginUpdateFirmWare(Device device)
		{
			var delayBytes = USBManager.Send(device, 0x39, 0x01);
			int delay = 0;
			if (delayBytes.Bytes.Count > 1)
				delay = (int)Math.Pow(2, delayBytes.Bytes[1]);
			return delay;
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

		// Подтверждение / завершение долговременной операции
		private static void ConfirmLongTermOperation(Device device)
		{
			var functionCode = USBManager.Send(device, 0x3C).FunctionCode;
			if (functionCode != 0x7c)
				ConfirmLongTermOperation(device);
		}
	}
}