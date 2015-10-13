using RubezhAPI.SKD;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
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