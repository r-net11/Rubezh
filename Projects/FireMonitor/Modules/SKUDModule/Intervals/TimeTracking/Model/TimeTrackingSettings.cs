using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDModule.Intervals.TimeTracking.Model
{
	public class TimeTrackingSettings
	{
		public TimeTrackingPeriod Period { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
