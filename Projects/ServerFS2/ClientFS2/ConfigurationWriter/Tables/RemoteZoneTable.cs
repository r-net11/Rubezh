using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class RemoteZoneTable : TableBase
	{
		public Zone Zone { get; set; }
		public ZonePanelItem ZonePanelItem { get; set; }

		public override Guid UID
		{
			get { return Zone.UID; }
		}

		public RemoteZoneTable(PanelDatabase2 panelDatabase, ZonePanelItem zonePanelItem)
			: base(panelDatabase, zonePanelItem.Zone.PresentationName)
		{
			Zone = zonePanelItem.Zone;
			ZonePanelItem = zonePanelItem;
		}

		public override void Create()
		{
			BytesDatabase.AddByte(0, "Внутренние параметры внешней зоны");
			BytesDatabase.AddShort((short)ZonePanelItem.No, "Номер локальной, для удаленного прибора, зоны");
			var localZones = ZonePanelRelations.GetAllZonesForPanel(ParentPanel, false);
			short localZoneNo = 0;
			var localZonePanelItem = localZones.FirstOrDefault(x => x.PanelDevice.UID == ParentPanel.UID);
			if (localZonePanelItem != null)
				localZoneNo = (short)localZonePanelItem.No;
			BytesDatabase.AddShort(localZoneNo, "Номер локальной зоны, с которой связано локальное ИУ");
			BytesDatabase.AddShort((short)ZonePanelItem.No, "Адрес удаленного прибора, ИП которого могут управлять локальными ИУ");
		}

		List<Zone> GetRemoteZonesForPanel(Device panelDevice)
		{
			var zones = new List<Zone>();
			foreach (var device in panelDevice.Children)
			{
				foreach (var zone in device.ZonesInLogic)
				{
					foreach (var deviceInZone in zone.DevicesInZone)
					{
						if (deviceInZone.ParentPanel.UID != panelDevice.UID)
						{
							if (!zones.Any(x => x.UID == zone.UID))
								zones.Add(zone);
						}
					}
				}
			}
			return zones;
		}
	}
}