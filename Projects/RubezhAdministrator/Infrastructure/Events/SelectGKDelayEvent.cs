using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace Infrastructure.Events
{
	public class SelectGKDelayEvent : CompositePresentationEvent<SelectGKDelayEventArg>
	{
	}

	public class SelectGKDelayEventArg
	{
		public bool Cancel { get; set; }
		public GKDelay Delay { get; set; }
	}
}