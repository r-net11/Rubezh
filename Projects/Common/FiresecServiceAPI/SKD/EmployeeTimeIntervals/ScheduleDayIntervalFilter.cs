using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ScheduleDayIntervalFilter : IsDeletedFilter
	{
		[DataMember]
		public Guid ScheduleSchemeUID { get; set; }
	}
}