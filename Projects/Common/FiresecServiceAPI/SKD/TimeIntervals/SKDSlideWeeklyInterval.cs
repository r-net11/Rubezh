using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDSlideWeeklyInterval
	{
		public SKDSlideWeeklyInterval()
		{
			WeeklyIntervalIDs = new List<int>();
		}

		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public bool IsDefault { get; set; }

		[DataMember]
		public List<int> WeeklyIntervalIDs { get; set; }
	}
}