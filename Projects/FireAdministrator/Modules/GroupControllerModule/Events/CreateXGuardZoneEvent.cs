using System;
using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateXGuardZoneEvent : CompositePresentationEvent<CreateXGuardZoneEventArg>
	{
	}

	public class CreateXGuardZoneEventArg
	{
		public bool Cancel { get; set; }
		public Guid ZoneUID { get; set; }
		public GKGuardZone Zone { get; set; }
	}
}