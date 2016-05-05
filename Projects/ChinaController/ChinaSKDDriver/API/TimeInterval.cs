using System;
using System.Collections.Generic;

namespace StrazhDeviceSDK.API
{
	public class TimeInterval
	{
		public TimeSpan StartDateTime { get; set; }

		public TimeSpan EndDateTime { get; set; }
	}

	public class NamedTimeInterval
	{
		public NamedTimeInterval()
		{
			Intervals = new List<TimeInterval>();
		}

		public List<TimeInterval> Intervals;
	}

	public class ControllerConfig
	{
		public ControllerConfig()
		{
			NamedTimeIntervals = new List<NamedTimeInterval>();
		}

		public List<NamedTimeInterval> NamedTimeIntervals;
	}
}