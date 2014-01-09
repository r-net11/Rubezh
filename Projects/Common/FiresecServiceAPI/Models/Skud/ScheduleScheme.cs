using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class ScheduleScheme
	{
		public Guid Uid { get; set; }
		public string Name { get; set; }
		public ScheduleSchemeType Type { get; set; }
		public List<Day> Days { get; set; }
	}

	public enum ScheduleSchemeType
	{
		Week,
		Shift,
		Month
	}
}
