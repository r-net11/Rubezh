using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class Schedule
	{
		public Guid Uid { get; set; }
		public string Name { get; set; }
		public ScheduleScheme ScheduleScheme { get; set; }
		public List<RegisterDevice> RegisterDevices { get; set; }
	}
}
