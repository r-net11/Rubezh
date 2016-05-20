using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace Infrastructure.Events
{
	public class SelectGKGuardZoneEvent : CompositePresentationEvent<SelectGKGuardZoneEventArg>
	{
	}

	public class SelectGKGuardZoneEventArg
	{
		public bool Cancel { get; set; }
		public GKGuardZone GuardZone { get; set; }
	}
}