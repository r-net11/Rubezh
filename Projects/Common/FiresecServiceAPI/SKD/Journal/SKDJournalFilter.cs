using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDJournalFilter : FilterBase
	{
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
		
		public SKDJournalFilter()
		{
			EventNames = new List<string>();
		}
	}
}