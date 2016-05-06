using System.Collections.Generic;

namespace StrazhDeviceSDK.API
{
	public class TimeShedule
	{
		public TimeShedule()
		{
			TimeSheduleIntervals = new List<TimeSheduleInterval>();
		}

		public List<TimeSheduleInterval> TimeSheduleIntervals { get; set; }
	}

	public class TimeSheduleInterval
	{
		public int BeginHours { get; set; }

		public int BeginMinutes { get; set; }

		public int BeginSeconds { get; set; }

		public int EndHours { get; set; }

		public int EndMinutes { get; set; }

		public int EndSeconds { get; set; }
	}
}