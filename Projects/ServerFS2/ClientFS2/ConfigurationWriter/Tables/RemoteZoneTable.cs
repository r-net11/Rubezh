using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecAPI.Models.Binary;

namespace ClientFS2.ConfigurationWriter
{
	public class RemoteZoneTable : TableBase
	{
		public Zone Zone { get; set; }
		public BinaryZone BinaryZone;

		public override Guid UID
		{
			get { return Zone.UID; }
		}

		public RemoteZoneTable(PanelDatabase2 panelDatabase, BinaryZone binaryZone)
			: base(panelDatabase, "Удаленная зона " + binaryZone.Zone.PresentationName)
		{
			binaryZone.IsRemote = true;
			BinaryZone = binaryZone;
			binaryZone.TableBase = this;
			Zone = binaryZone.Zone;
		}

		public override void Create()
		{
			BytesDatabase.AddByte(0, "Внутренние параметры внешней зоны", true);
			BytesDatabase.AddByte(0, "Внутренние параметры внешней зоны", true);
			BytesDatabase.AddByte(0, "Внутренние параметры внешней зоны", true);
			BytesDatabase.AddByte(0, "Внутренние параметры внешней зоны", true);

			var localBinaryPanel = BinaryZone.BinaryPanels.FirstOrDefault(x => x.ParentPanel.UID == ParentPanel.UID);
			var localBinaryZone = localBinaryPanel.BinaryLocalZones.FirstOrDefault(x => x.Zone.UID == Zone.UID);

			var localZoneNo = 0;
			if (localBinaryZone != null)
				localZoneNo = localBinaryZone.LocalNo;

			var remoteZoneNo = 0;
			BinaryZone remoteBinaryZone = null;
			//var remoteBinaryPanel = BinaryZone.BinaryPanels.FirstOrDefault(x => x.ParentPanel.UID != ParentPanel.UID);
			var remoteBinaryPanel = BinaryZone.BinaryPanels.FirstOrDefault(x => x.ParentPanel.UID == BinaryZone.ParentPanel.UID);
			if (Zone.No == 8)
			{
				var panelAddess = remoteBinaryPanel.ParentPanel.IntAddress;
			}
			if (remoteBinaryPanel != null)
			{
				remoteBinaryZone = remoteBinaryPanel.BinaryLocalZones.FirstOrDefault(x => x.Zone.UID == Zone.UID);
				if (remoteBinaryZone != null)
				{
					remoteZoneNo = remoteBinaryZone.LocalNo;
				}
			}

			BytesDatabase.AddShort((short)remoteZoneNo, "Номер локальной, для удаленного прибора, зоны");
			BytesDatabase.AddShort((short)localZoneNo, "Номер локальной зоны, с которой связано локальное ИУ");
			BytesDatabase.AddByte((byte)BinaryZone.ParentPanel.IntAddress, "Адрес удаленного прибора, ИП которого могут управлять локальными ИУ по логике межприборное И");
		}
	}
}