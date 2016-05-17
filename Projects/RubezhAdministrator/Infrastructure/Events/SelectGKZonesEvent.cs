using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SelectGKZonesEvent : CompositePresentationEvent<SelectGKZonesEventArg>
	{
	}

	public class SelectGKZonesEventArg
	{
		public bool Cancel { get; set; }
		public List<GKZone> Zones { get; set; }
	}
}