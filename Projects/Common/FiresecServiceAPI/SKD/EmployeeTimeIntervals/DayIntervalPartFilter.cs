using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class DayIntervalPartFilter : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> DayIntervalUIDs { get; set; }
	}
}