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
			USBManager.Send(device, 0x01, 0x04);
			USBManager.Send(device.ParentUSB, 0x02, 0x34, 0x01); // Запись в MDS
			USBManager.Send(device.ParentUSB, 0x02, 0x37); // Информация о прошивке
			USBManager.Send(device, 0x01, 0x04);
			ConfirmLongTermOperation(device);
			USBManager.Send(device.ParentUSB, 0x01, 0x36);
			ConfirmLongTermOperation(device);
			USBManager.Send(device, 0x01, 0x01);
			USBManager.Send(device, 0x01, 0x03);
			USBManager.Send(device, 0x01, 0x24);
			USBManager.Send(device, 0x01, 0x51);
			USBManager.Send(device, 0x01, 0x57);
			USBManager.Send(device, 0x01, 0x01);
			BlockBD(device);
			USBManager.Send(device, 0x01, 0x14); // Интервал тишины?
			USBManager.Send(device, 0x01, 0x14);
			USBManager.Send(device, 0x01, 0x57);
			WriteFlashConfiguration(device, Flash);
			USBManager.Send(device, 0x01, 0x01);
			//USBManager.SendCodeToPanel(device, 0x01, 0x01);
			var delayBytes = USBManager.Send(device, 0x39, 0x01);
			int delay = 0;
			if(delayBytes.Bytes.Count > 1)
				delay = (int)Math.Pow(2, delayBytes.Bytes[1]);
			Thread.Sleep(delay);
			ConfirmLongTermOperation(device);
			USBManager.Send(device, 0x3D);
			USBManager.Send(device, 0x01, 0x01);
			var firmwareFileName = Path.Combine(AppDataFolderHelper.GetFolder("Server"), "frm.fscf");
			var hexInfo = FirmwareUpdateOperationHelper.GetHexInfo(firmwareFileName, device.Driver.ShortName + ".hex");
			WriteRomConfiguration(device, hexInfo.Bytes, hexInfo.Offset);
			delayBytes = USBManager.Send(device, 0x39, 0x04);
			if (delayBytes.Bytes.Count > 1)
				delay = (int)Math.Pow(2, delayBytes.Bytes[1]);
			Thread.Sleep(delay);
			ConfirmLongTermOperation(device);
			USBManager.Send(device, 0x3D);
			USBManager.Send(device, 0x01, 0x01);
			ClearSector(device);
			ConfirmLongTermOperation(device);
			USBManager.Send(device, 0x3D);
			WriteRomConfiguration(device, Rom, romDBFirstIndex);
			StopUpdating(device);
			ConfirmLongTermOperation(device);
			USBManager.Send(device, 0x01, 0x01);
			//USBManager.SendCodeToPanel(device, 0x01, 0x01);
			//USBManager.SendCodeToPanel(device, 0x01, 0x01);
			//USBManager.SendCodeToPanel(device, 0x01, 0x01);
			//USBManager.SendCodeToPanel(device, 0x01, 0x01);
			//USBManager.SendCodeToPanel(device, 0x01, 0x01);
			ServerHelper.SynchronizeTime(device);
			USBManager.Send(device.ParentUSB, 0x02, 0x34, 0x01); // Запись в MDS
			USBManager.Send(device.ParentUSB, 0x02, 0x37); // Информация о прошивке
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
		}

		// Очистка сектора памяти bSectorStart, bSectorEnd
		private static void ClearSector(Device device)
		{
			USBManager.Send(device, 0x3B, 0x03, 0x04);
		}

		// Подтверждение / завершение долговременной операции
		private static void ConfirmLongTermOperation(Device device)
		{
			USBManager.Send(device, 0x3C);
		}
	}
}