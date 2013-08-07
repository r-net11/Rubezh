using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServerFS2.Helpers
{
	[DataContract]
	public class DeviceLastIndexCollection
	{
		public DeviceLastIndexCollection()
		{
			DeviceLastJournalIndexes = new List<DeviceLastJournalIndex>();
		}

		[DataMember]
		public List<DeviceLastJournalIndex> DeviceLastJournalIndexes { get; set; }
	}

	[DataContract]
	public class DeviceLastJournalIndex
	{
		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public int LastFireJournalIndex { get; set; }

		[DataMember]
		public int LastSecurityJournalIndex { get; set; }
	}
}