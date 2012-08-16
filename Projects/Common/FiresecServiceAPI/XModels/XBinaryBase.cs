using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public abstract class XBinaryBase
	{
		public XBinaryBase()
		{
			ClearBinaryData();
		}

		public List<XBinaryBase> InputObjects { get; set; }
		public List<XBinaryBase> OutputObjects { get; set; }

		public XDevice KauDatabaseParent { get; set; }
		public XDevice GkDatabaseParent { get; set; }

		ushort databaseGKNo;
		ushort databaseKAUNo;

		public void ClearBinaryData()
		{
			InputObjects = new List<XBinaryBase>();
			OutputObjects = new List<XBinaryBase>();
		}

		public ushort GetDatabaseNo(DatabaseType databaseType)
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

		public void SetDatabaseNo(DatabaseType databaseType, ushort no)
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