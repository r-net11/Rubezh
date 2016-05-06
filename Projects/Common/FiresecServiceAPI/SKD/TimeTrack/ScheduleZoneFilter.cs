using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ScheduleZoneFilter : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> ScheduleUIDs { get; set; }
	}
}