using System;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class PDUPTTable : TableBase
	{
		public Device Device { get; set; }

		public PDUPTTable(DevicePDUDirection devicePDUDirection, PanelDatabase panelDatabase)
			: base(null, devicePDUDirection.PDUGroupDevice.Device.DottedPresentationNameAndAddress)
		{
			Device = devicePDUDirection.PDUGroupDevice.Device;

			BytesDatabase = new BytesDatabase(Device.DottedPresentationNameAndAddress);
			BytesDatabase.AddByte(Device.AddressOnShleif, "Адрес");
			BytesDatabase.AddByte((Device.ShleifNo - 1), "Шлейф");
			BytesDatabase.AddByte(Device.Parent.IntAddress, "Адрес прибора");
			var deviceCode = FiresecAPI.Models.DriversHelper.GetCodeForFriver(Device.Driver.DriverType);
			BytesDatabase.AddByte(deviceCode, "Тип ИУ");

			var tableBase = panelDatabase.FlashDatabase.LocalZonesTableGroup.Tables.FirstOrDefault(x => x.UID == Device.Zone.UID);
			var localZoneNo = (tableBase as ZoneTable).BinaryZone.LocalNo;
			BytesDatabase.AddShort(localZoneNo, "Номер зоны");
			BytesDatabase.AddByte(devicePDUDirection.Device.IntAddress, "Направление");

			foreach (var tableGroup in panelDatabase.FlashDatabase.DevicesTableGroups)
			{
				tableBase = tableGroup.Tables.FirstOrDefault(x => x.UID == Device.UID);
				if (tableBase != null)
				{
					break;
				}
			}
			var offset = tableBase.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset + 3;
			var offsetBytes = BitConverter.GetBytes(offset);
			for (int i = 0; i < 4; i++)
			{
				BytesDatabase.AddByte(offsetBytes[i], "Смещение");
			}
		}
	}
}