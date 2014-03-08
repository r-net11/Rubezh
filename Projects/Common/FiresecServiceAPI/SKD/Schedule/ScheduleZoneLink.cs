using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI
{
	[DataContract]
	public class ScheduleZoneLink : SKDModelBase
	{

		[DataMember]
		public Guid? ScheduleUid { get; set; }

		[DataMember]
		public Guid? ZoneUid { get; set; }

		[DataMember]
		public bool? CanControl { get; set; }
	}
}