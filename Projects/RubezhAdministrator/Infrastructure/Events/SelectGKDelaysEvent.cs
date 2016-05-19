using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SelectGKDelaysEvent : CompositePresentationEvent<SelectGKDelaysEventArg>
	{
	}

	public class SelectGKDelaysEventArg
	{
		public bool Cancel { get; set; }
		public List<GKDelay> Delays { get; set; }
	}
}