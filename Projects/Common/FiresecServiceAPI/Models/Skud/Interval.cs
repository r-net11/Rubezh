using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class Interval
	{
		public Guid Uid { get; set; }
		public DateTime BeginDate { get; set; }
		public DateTime EndDate { get; set; }
		public Transition Transition { get; set; }
	}

	public enum Transition
	{
		Day,
		Night,
		DayNight
	}
}
