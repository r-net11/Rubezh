using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class PDUPTTable : TableBase
	{
		public Device Device { get; set; }

		public PDUPTTable(DevicePDUDirection devicePDUDirection, PanelDatabase panelDatabase)
			: base(null, devicePDUDirection.PDUGroupDevice.Device.DottedPresentationNameAndAddress)
		{
			Device = devicePDUDirection.PDUGroupDevice.Device;

			BytesDatabase = new BytesDatabase(Device.DottedPresentationNameAndAddress);
			BytesDatabase.AddByte((byte)Device.AddressOnShleif, "Адрес");
			BytesDatabase.AddByte((byte)(Device.ShleifNo - 1), "Шлейф");
			BytesDatabase.AddByte((byte)Device.Parent.IntAddress, "Адрес прибора");
			var deviceCode = DriversHelper.GetCodeForFriver(Device.Driver.DriverType);
			BytesDatabase.AddByte((byte)deviceCode, "Тип ИУ");

			var tableBase = panelDatabase.PanelDatabase2.LocalZonesTableGroup.Tables.FirstOrDefault(x => x.UID == Device.Zone.UID);
			var localZoneNo = (tableBase as ZoneTable).BinaryZone.LocalNo;
			BytesDatabase.AddShort((short)localZoneNo, "Номер зоны");
			BytesDatabase.AddByte((byte)devicePDUDirection.Device.IntAddress, "Направление");

			foreach (var tableGroup in panelDatabase.PanelDatabase2.DevicesTableGroups)
			{
				tableBase = tableGroup.Tables.FirstOrDefault(x => x.UID == Device.UID);
				if (tableBase != null)
				{
					break;
				}
			}
			var offset = tableBase.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
			var offsetBytes = BitConverter.GetBytes(offset);
			for (int i = 0; i < 4; i++)
			{
				BytesDatabase.AddByte(offsetBytes[i], "Смещение");
			}
		}
	}
}