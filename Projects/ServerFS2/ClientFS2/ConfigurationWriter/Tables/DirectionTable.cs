using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class DirectionTable : TableBase
	{
		public Direction Direction { get; set; }

		public override Guid UID
		{
			get { return Direction.UID; }
		}

		public DirectionTable(PanelDatabase2 panelDatabase2, Direction direction)
			: base(panelDatabase2, direction.Name)
		{
			Direction = direction;
		}

		public override void Create()
		{
			BytesDatabase.AddString(Direction.Name, "Имя");
			BytesDatabase.AddByte(0, "Резерв");

			var localValves = new List<Device>();
			foreach (var zoneUID in Direction.ZoneUIDs)
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

			foreach (var valveDevice in localValves)
			{
				var table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == valveDevice.UID);
				BytesDatabase.AddReferenceToTable(table, "Указатель на локальную задвижку");
			}

			var rmShleif = 0;
			var rmAddress = 0;
			if (Direction.DeviceRm != Guid.Empty)
			{
				var rmDevice = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == Direction.DeviceRm);
				if (rmDevice != null)
				{
					rmShleif = rmDevice.ShleifNo;
					rmAddress = rmDevice.AddressOnShleif;
				}
				BytesDatabase.AddByte((byte)rmShleif, "Шлейф РМ с внешней сигнализацией УАПТ");
				BytesDatabase.AddByte((byte)rmAddress, "Адрес РМ с внешней сигнализацией УАПТ");
			}
		}
	}
}