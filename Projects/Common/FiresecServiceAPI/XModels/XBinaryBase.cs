using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XBinaryBase
	{
		public List<XDevice> InputDevices { get; set; }
		public List<XDevice> OutputDevices { get; set; }
		public List<XZone> InputZones { get; set; }
		public List<XZone> OutputZones { get; set; }

		public XDevice KauDatabaseParent { get; set; }
		public XDevice GkDatabaseParent { get; set; }

		short databaseGKNo;
		short databaseKAUNo;

		public void ClearBinaryData()
		{
			InputDevices = new List<XDevice>();
			OutputDevices = new List<XDevice>();
			InputZones = new List<XZone>();
			OutputZones = new List<XZone>();
		}

		public short GetDatabaseNo(DatabaseType databaseType)
		{
			switch (databaseType)
			{
				case DatabaseType.Gk:
					return databaseGKNo;

				case DatabaseType.Kau:
					return databaseKAUNo;
			}
			return 0;
		}

		public void SetDatabaseNo(DatabaseType databaseType, short no)
		{
			switch (databaseType)
			{
				case DatabaseType.Gk:
					databaseGKNo = no;
					break;

				case DatabaseType.Kau:
					databaseKAUNo = no;
					break;
			}
		}
	}
}