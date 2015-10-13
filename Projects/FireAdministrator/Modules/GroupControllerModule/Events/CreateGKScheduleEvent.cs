using System;
using RubezhAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateGKScheduleEvent : CompositePresentationEvent<CreateGKScheduleEventArg>
	{
	}

	public class CreateGKScheduleEventArg
	{
		public bool Cancel { get; set; }
		public Guid ScheduleUID { get; set; }
		public GKSchedule Schedule { get; set; }
	}
}