using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKModule
{
	public class Alarm
	{
		public string Name { get; set; }
		public XAlarmType AlarmType { get; set; }
		public XStateType StateType { get; set; }
		public XDevice Device { get; set; }
		public XZone Zone { get; set; }
		public XDirection Direction { get; set; }
	}
}