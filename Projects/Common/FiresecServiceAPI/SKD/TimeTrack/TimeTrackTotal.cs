using System;

namespace StrazhAPI.SKD
{
	public class TimeTrackTotal
	{
		public TimeTrackTotal(TimeTrackType timeTrackType)
		{
			TimeTrackType = timeTrackType;
		}

		public TimeTrackTotal()
		{
		}

		public TimeTrackType TimeTrackType { get; set; }

		public TimeSpan TimeSpan { get; set; }

		public static TimeSpan operator +(TimeTrackTotal t1, TimeTrackTotal t2)
		{
			if(t1 == null && t2 == null) return new TimeSpan();
			if (t1 == null) return t2.TimeSpan;
			if (t2 == null) return t1.TimeSpan;

			return t1.TimeSpan + t2.TimeSpan;
		}
	}
}