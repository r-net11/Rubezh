using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecAPI.Journal;

namespace FiresecAPI.Journal
{
	[DataContract]
	public class JournalFilter : FilterBase
	{
		public JournalFilter()
		{
			UID = Guid.NewGuid();
			SystemDateTime = new DateTimePeriod();
			DeviceDateTime = new DateTimePeriod();
			JournalEventNameTypes = new List<JournalEventNameType>();
			JournalSubsystemTypes = new List<JournalSubsystemType>();
			DeviceUIDs = new List<Guid>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DateTimePeriod SystemDateTime { get; set; }

		[DataMember]
		public DateTimePeriod DeviceDateTime { get; set; }

		[DataMember]
		public List<JournalEventNameType> JournalEventNameTypes { get; set; }

		[DataMember]
		public List<JournalSubsystemType> JournalSubsystemTypes { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }
	}
}