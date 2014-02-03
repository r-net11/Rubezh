using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class DateTimePeriod
	{
		[DataMember]
		public DateTime StartDate { get; set; }
		[DataMember]
		public DateTime EndDate { get; set; }
	}
}


