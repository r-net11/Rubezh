using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.SKD
{
	public class TimeTrackTotal
	{
		public TimeTrackTotal(TimeTrackType timeTrackType)
		{
			TimeTrackType = timeTrackType;
		}

		public TimeTrackType TimeTrackType { get; set; }
		public TimeSpan TimeSpan { get; set; }
	}
}