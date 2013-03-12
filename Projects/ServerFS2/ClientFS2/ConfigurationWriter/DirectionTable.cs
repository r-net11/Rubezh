using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class DirectionTable : TableBase
	{
		public DirectionTable(PanelDatabase panelDatabase, Direction direction)
			: base(panelDatabase)
		{
			BytesDatabase.AddString(direction.Name, "Имя");
			BytesDatabase.AddByte(0, "Резерв");

			var localValves = new List<Device>();
			foreach (var zoneUID in direction.ZoneUIDs)
			{
				var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
				if (zone != null)
				{
					foreach (var device in zone.DevicesInZoneLogic)
					{
						if (device.Driver.DriverType == DriverType.Valve && device.ParentPanel.UID == ParentPanel.UID)
							localValves.Add(device);
					}
				}
			}

			foreach (var valve in localValves)
			{
				BytesDatabase.AddReference(null, "Указатель на локальную задвижку");
			}

			var rmShleif = 0;
			var rmAddress = 0;
			if (direction.DeviceRm != Guid.Empty)
			{
				var rmDevice = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == direction.DeviceRm);
				if (rmDevice != null)
				{
					rmShleif = rmDevice.ShleifNo;
					rmAddress = rmDevice.IntAddress / 256;
				}
				BytesDatabase.AddByte((byte)rmShleif, "Шлейф РМ с внешней сигнализацией УАПТ");
				BytesDatabase.AddByte((byte)rmAddress, "Адрес РМ с внешней сигнализацией УАПТ");
			}
			BytesDatabase.SetGroupName(direction.Name);
		}
	}
}