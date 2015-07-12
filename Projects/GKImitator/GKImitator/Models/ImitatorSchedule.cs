using FiresecAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GKImitator.Processor
{
	[DataContract]
	public class ImitatorSchedule
	{
		public ImitatorSchedule()
		{
			ImitatorSheduleIntervals = new List<ImitatorSheduleInterval>();
		}

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int HolidayScheduleNo { get; set; }

		[DataMember]
		public int PartsCount { get; set; }

		[DataMember]
		public int TotalSeconds { get; set; }

		[DataMember]
		public int WorkHolidayScheduleNo { get; set; }

		[DataMember]
		public List<ImitatorSheduleInterval> ImitatorSheduleIntervals { get; set; }
	}

	[DataContract]
	public class ImitatorSheduleInterval
	{
		[DataMember]
		public int StartSeconds { get; set; }

		[DataMember]
		public int EndSeconds { get; set; }
	}
}