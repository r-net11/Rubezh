using System;
using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateGKGuardZoneEvent : CompositePresentationEvent<CreateGKGuardZoneEventArg>
	{
	}

	public class CreateGKGuardZoneEventArg
	{
		public bool Cancel { get; set; }
		public Guid ZoneUID { get; set; }
		public GKGuardZone Zone { get; set; }
	}
}