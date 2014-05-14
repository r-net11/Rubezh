using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI
{
	[DataContract]
	public class SKDJournalFilter : FilterBase
	{
		public SKDJournalFilter()
		{
			UID = Guid.NewGuid();
			SystemDateTime = new DateTimePeriod();
			DeviceDateTime = new DateTimePeriod();
			EventNames = new List<string>();
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
		public List<string> EventNames { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }
	}
}