﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class DirectionTable : TableBase
	{
		public Direction Direction { get; set; }

		public override Guid UID
		{
			get { return Direction.UID; }
		}

		public DirectionTable(FlashDatabase flashDatabase, Direction direction)
			: base(flashDatabase, direction.Name)
		{
			Direction = direction;
		}

		public override void Create()
		{
			BytesDatabase.AddString(Direction.Name, "Имя");
			BytesDatabase.AddByte(1, "Резерв");

			var localValves = new HashSet<Device>();
			foreach (var zoneUID in Direction.ZoneUIDs)
			{
				var zone = ConfigurationManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
				if (zone != null)
				{
					foreach (var device in zone.DevicesInZoneLogic)
					{
						if (device.Driver.DriverType == DriverType.Valve && device.ParentPanel.UID == ParentPanel.UID)
							localValves.Add(device);
					}
				}
			}

            BytesDatabase.AddShort(localValves.Count, "Количество локальных задвижек в зонах направления");
			foreach (var valveDevice in localValves)
			{
				var table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == valveDevice.UID);
				BytesDatabase.AddReferenceToTable(table, "Указатель на локальную задвижку");
			}

			var rmShleif = 0;
			var rmAddress = 0;
			if (Direction.DeviceRm != Guid.Empty)
			{
				var rmDevice = ConfigurationManager.Devices.FirstOrDefault(x => x.UID == Direction.DeviceRm);
				if (rmDevice != null)
				{
					rmShleif = rmDevice.ShleifNo - 1;
					rmAddress = rmDevice.AddressOnShleif;
				}
				BytesDatabase.AddByte(rmShleif, "Шлейф РМ с внешней сигнализацией УАПТ");
				BytesDatabase.AddByte(rmAddress, "Адрес РМ с внешней сигнализацией УАПТ");
			}
		}
	}
}