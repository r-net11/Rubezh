using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ScheduleZoneLink : SKDIsDeletedModel
	{

		[DataMember]
		public Guid? ScheduleUid { get; set; }

		[DataMember]
		public Guid? ZoneUid { get; set; }

		[DataMember]
		public bool? CanControl { get; set; }
	}
}