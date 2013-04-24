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
		BinaryZone BinaryZone;

		public override Guid UID
		{
			get { return Zone.UID; }
		}

		public RemoteZoneTable(PanelDatabase2 panelDatabase, BinaryZone binaryZone)
			: base(panelDatabase, binaryZone.Zone.PresentationName)
		{
			BinaryZone = binaryZone;
			binaryZone.TableBase = this;
			Zone = binaryZone.Zone;
		}

		public override void Create()
		{
			BytesDatabase.AddByte(0, "Внутренние параметры внешней зоны", true);
			BytesDatabase.AddShort((short)BinaryZone.LocalNo, "Номер локальной, для удаленного прибора, зоны");
			short localZoneNo = 0;
			BytesDatabase.AddShort(localZoneNo, "Номер локальной зоны, с которой связано локальное ИУ");
			BytesDatabase.AddShort((short)BinaryZone.ParentPanel.IntAddress, "Адрес удаленного прибора, ИП которого могут управлять локальными ИУ");
		}
	}
}