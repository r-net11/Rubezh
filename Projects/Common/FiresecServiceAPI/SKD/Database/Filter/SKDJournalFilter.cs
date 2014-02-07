using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDJournalFilter : FilterBase
	{
		[DataMember]
		public List<Guid> Uids { get; set; }

		[DataMember]
		public DateTimePeriod SystemDateTime { get; set; }

		[DataMember]
		public DateTimePeriod DeviceDateTime { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<string> EventNames { get; set; }

		public bool HasUids
		{
			get { return Uids.Count > 0; }
		}

		public SKDJournalFilter()
		{
			Uids = new List<Guid>();
			EventNames = new List<string>();
		}
	}
}