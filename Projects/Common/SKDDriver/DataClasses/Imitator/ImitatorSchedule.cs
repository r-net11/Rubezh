using FiresecAPI.GK;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SKDDriver.DataClasses
{
	public class ImitatorSchedule
	{
		public ImitatorSchedule()
		{
			UID = Guid.NewGuid();
			ImitatorSheduleIntervals = new List<ImitatorSheduleInterval>();
		}

		[Key]
		public Guid UID { get; set; }

		public int No { get; set; }

		[MaxLength(32)]
		public string Name { get; set; }

		public int HolidayScheduleNo { get; set; }

		public int PartsCount { get; set; }

		public int TotalSeconds { get; set; }

		public int WorkHolidayScheduleNo { get; set; }

		public List<ImitatorSheduleInterval> ImitatorSheduleIntervals { get; set; }
	}

	public class ImitatorSheduleInterval
	{
		public ImitatorSheduleInterval()
		{
			UID = Guid.NewGuid();
		}

		[Key]
		public Guid UID { get; set; }

		public int StartSeconds { get; set; }

		public int EndSeconds { get; set; }

		public string StartDateTime
		{
			get
			{
				var result = new DateTime(2000, 1, 1).AddSeconds(StartSeconds);
				return result.ToString();
			}
		}

		public string EndDateTime
		{
			get
			{
				var result = new DateTime(2000, 1, 1).AddSeconds(EndSeconds);
				return result.ToString();
			}
		}
	}
}