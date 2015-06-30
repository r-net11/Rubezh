using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;
using FiresecAPI.GK;
using System;

namespace GKModule.Events
{
	public class CreateGKSKDZoneEvent : CompositePresentationEvent<CreateGKSKDZoneEventArg>
	{
	}

	public class CreateGKSKDZoneEventArg
	{
		public bool Cancel { get; set; }
		public Guid ZoneUID { get; set; }
		public GKSKDZone Zone { get; set; }
	}
}