using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace Infrastructure.Events
{
	public class SelectGKZoneEvent : CompositePresentationEvent<SelectGKZoneEventArg>
	{
	}

	public class SelectGKZoneEventArg
	{
		public bool Cancel { get; set; }
		public GKZone Zone { get; set; }
	}
}