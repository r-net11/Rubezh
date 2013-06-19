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
			for (int i = 0; i < deviceRom.Count; i = i + 0x100)
			{
				USBManager.SendCodeToPanel(device, 0x3E, BitConverter.GetBytes(begin + i).Reverse(), deviceRom.GetRange(i, Math.Min(deviceRom.Count - i, 0x100)));
			}
		}

		public static void SetDeviceConfig(Device device, List<byte> Rom, List<byte> Flash)
		{
			var panelDatabaseReader = new ReadPanelDatabaseOperationHelper(false);
			var romDBFirstIndex = panelDatabaseReader.GetRomFirstIndex(device);
			Rom = panelDatabaseReader.GetRomDBBytes(device);
			Flash = panelDatabaseReader.GetFlashDBBytes(device);

			//SendCode(CreateBytesArray(0x01, 0x02, 0x34, 0x01)); // Запись в MDS
			//SendCode(CreateBytesArray(0x01, 0x02, 0x37)); // Информация о прошивке
			BlockBD(device);
			SetFlashConfig(device, Flash);
			var delay = (int)Math.Pow(2, USBManager.SendCodeToPanel(device, 0x39, 0x01).Bytes[0]);
			Thread.Sleep(delay);
			ConfirmationLongTermOperation(device);

			var firmwareFileName = Path.Combine(AppDataFolderHelper.GetFolder("Server"), "frm.fscf");
			var hexInfo = FirmwareUpdateOperationHelper.GetHexInfo(firmwareFileName, device.Driver.ShortName + ".hex");
			SetRomConfig(device, hexInfo.Bytes, hexInfo.Offset);

			delay = (int)Math.Pow(2, USBManager.SendCodeToPanel(device, 0x39, 0x04).Bytes[0]);
			ConfirmationLongTermOperation(device);
			ClearSector(device);
			Thread.Sleep(delay);
			SetRomConfig(device, Rom, romDBFirstIndex);
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