using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ScheduleZone : SKDModelBase
	{
		[DataMember]
		public Guid ScheduleUID { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }
	}
}