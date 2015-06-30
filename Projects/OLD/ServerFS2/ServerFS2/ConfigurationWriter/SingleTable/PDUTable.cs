using System;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class PDUTable : TableBase
	{
		public Device Device { get; set; }

		public PDUTable(DevicePDUDirection devicePDUDirection, PanelDatabase panelDatabase)
			: base(null, devicePDUDirection.PDUGroupDevice.Device.DottedPresentationNameAndAddress)
		{
			Device = devicePDUDirection.PDUGroupDevice.Device;
			BytesDatabase = new BytesDatabase(Device.DottedPresentationNameAndAddress);
			BytesDatabase.AddByte(Device.AddressOnShleif, "Адрес");
			BytesDatabase.AddByte((Device.ShleifNo - 1), "Шлейф");
			BytesDatabase.AddByte(Device.Parent.IntAddress, "Адрес прибора");
			var deviceCode = FiresecAPI.Models.DriversHelper.GetCodeForDriver(Device.Driver.DriverType);
			BytesDatabase.AddByte(deviceCode, "Тип ИУ");
			var option = 0;
			if (devicePDUDirection.PDUGroupDevice.IsInversion)
				option = 128;
			var anotherCount = devicePDUDirection.Device.Parent.Children.Count(x => x.PDUGroupLogic.Devices.Any(y => y.DeviceUID == Device.UID));
			if (anotherCount > 1)
				option += 64;
			BytesDatabase.AddByte(option, "Опции");
			BytesDatabase.AddByte(devicePDUDirection.Device.IntAddress, "Направление");
			BytesDatabase.AddByte(0, "Пустой байт");

			TableBase tableBase = null;
			foreach (var tableGroup in panelDatabase.FlashDatabase.DevicesTableGroups)
			{
				tableBase = tableGroup.Tables.FirstOrDefault(x => x.UID == Device.UID);
				if (tableBase != null)
				{
					break;
				}
			}
			var offset = tableBase.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
			if (Device.Driver.IsZoneLogicDevice)
			{
				offset += 3;
			}
			else
			{
				offset += 2;
			}
			var offsetBytes = BitConverter.GetBytes(offset);
			for (int i = 0; i < 4; i++)
			{
				BytesDatabase.AddByte(offsetBytes[i], "Смещение");
			}

			BytesDatabase.AddByte(devicePDUDirection.PDUGroupDevice.OnDelay, "Задержка на включение");
			BytesDatabase.AddByte(devicePDUDirection.PDUGroupDevice.OffDelay, "Задержка на выключение");
			for (int i = 0; i < 9; i++)
			{
				BytesDatabase.AddByte(255, "ПДУ привязки");
			}
			BytesDatabase.AddByte(0, "ПДУ привязки");
		}
	}
}