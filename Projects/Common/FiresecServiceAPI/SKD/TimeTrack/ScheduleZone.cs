using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ScheduleZone : SKDModelBase
	{
		[DataMember]
		public Guid ScheduleUID { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public Guid DoorUID { get; set; }
	}
}