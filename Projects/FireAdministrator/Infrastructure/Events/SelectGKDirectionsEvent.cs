using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SelectGKDirectionsEvent : CompositePresentationEvent<SelectGKDirectionsEventArg>
	{
	}

	public class SelectGKDirectionsEventArg
	{
		public bool Cancel { get; set; }
		public List<GKDirection> Directions { get; set; }
	}
}