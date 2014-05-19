using System;
using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateXZoneEvent : CompositePresentationEvent<CreateXZoneEventArg>
	{
	}

	public class CreateXZoneEventArg
	{
		public bool Cancel { get; set; }
		public Guid ZoneUID { get; set; }
		public XZone Zone { get; set; }
	}
}