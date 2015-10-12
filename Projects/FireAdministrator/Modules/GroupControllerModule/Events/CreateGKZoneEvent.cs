using System;
using RubezhAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateGKZoneEvent : CompositePresentationEvent<CreateGKZoneEventArg>
	{
	}

	public class CreateGKZoneEventArg
	{
		public bool Cancel { get; set; }
		public Guid ZoneUID { get; set; }
		public GKZone Zone { get; set; }
	}
}