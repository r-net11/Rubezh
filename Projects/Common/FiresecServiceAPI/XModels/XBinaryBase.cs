using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public abstract class XBinaryBase
	{
		public List<XBinaryBase> InputObjects { get; set; }
		public List<XBinaryBase> OutputObjects { get; set; }

		public XDevice KauDatabaseParent { get; set; }
		public XDevice GkDatabaseParent { get; set; }

		short databaseGKNo;
		short databaseKAUNo;

		public void ClearBinaryData()
		{
			InputObjects = new List<XBinaryBase>();
			OutputObjects = new List<XBinaryBase>();
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

		public abstract XBinaryInfo BinaryInfo { get; }
		public abstract string GetBinaryDescription();
	}
}