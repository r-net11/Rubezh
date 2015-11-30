using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SelectGKGuardZonesEvent : CompositePresentationEvent<SelectGKGuardZonesEventArg>
	{
	}

	public class SelectGKGuardZonesEventArg
	{
		public bool Cancel { get; set; }
		public List<GKGuardZone> GuardZones { get; set; }
	}
}