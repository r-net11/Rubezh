using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class BinaryDirection
	{
		public BinaryDirection(Device panelDevice)
		{
			var localDirections = new List<Direction>();
			foreach (var direction in ConfigurationManager.DeviceConfiguration.Directions)
			{
				foreach (var zoneUID in direction.ZoneUIDs)
				{
					var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						if (ZonePanelRelations.IsLocalZone(zone, panelDevice))
							localDirections.Add(direction);
					}
				}
			}
			foreach (var direction in localDirections)
			{
				AddBinaryDirection(panelDevice, direction);
			}
		}

		public void AddBinaryDirection(Device panelDevice, Direction direction)
		{
			var bytesDatabase = new BytesDatabase();
			bytesDatabase.AddString(direction.Name, "Имя");
			bytesDatabase.AddByte(0, "Резерв");

			var localValves = new List<Device>();
			foreach (var zoneUID in direction.ZoneUIDs)
			{
				var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
				if (zone != null)
				{
					foreach (var device in zone.DevicesInZoneLogic)
					{
						if (device.Driver.DriverType == DriverType.Valve && device.ParentPanel.UID == panelDevice.UID)
							localValves.Add(device);
					}
				}
			}

			foreach (var valve in localValves)
			{
				bytesDatabase.AddReference(null, "Указатель на локальную задвижку");
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
				bytesDatabase.AddByte((byte)rmShleif, "Шлейф РМ с внешней сигнализацией УАПТ");
				bytesDatabase.AddByte((byte)rmAddress, "Адрес РМ с внешней сигнализацией УАПТ");
			}
		}
	}
}