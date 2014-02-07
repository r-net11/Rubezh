using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Schedule
	{
		public Schedule()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ScheduleScheme ScheduleScheme { get; set; }

		[DataMember]
		public List<RegisterDevice> RegisterDevices { get; set; }
	}
}