using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XBinaryBase
	{
		public XDevice KauDatabaseParent { get; set; }
		public XDevice GkDatabaseParent { get; set; }

		short InternalGKNo { get; set; }
		short InternalKAUNo { get; set; }

		public short GetDatabaseNo(DatabaseType databaseType)
		{
			switch (databaseType)
			{
				case DatabaseType.Gk:
					return InternalGKNo;

				case DatabaseType.Kau:
					return InternalKAUNo;
			}
			return 0;
		}

		public void SetDatabaseNo(DatabaseType databaseType, short no)
		{
			switch (databaseType)
			{
				case DatabaseType.Gk:
					InternalGKNo = no;
					break;

				case DatabaseType.Kau:
					InternalKAUNo = no;
					break;
			}
		}
	}
}