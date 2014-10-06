using System;
using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateXScheduleEvent : CompositePresentationEvent<CreateXScheduleEventArg>
	{
	}

	public class CreateXScheduleEventArg
	{
		public bool Cancel { get; set; }
		public Guid ScheduleUID { get; set; }
		public GKSchedule Schedule { get; set; }
	}
}