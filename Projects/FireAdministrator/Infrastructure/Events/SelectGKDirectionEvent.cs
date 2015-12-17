using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace Infrastructure.Events
{
	public class SelectGKDirectionEvent : CompositePresentationEvent<SelectGKDirectionEventArg>
	{
	}

	public class SelectGKDirectionEventArg
	{
		public bool Cancel { get; set; }
		public GKDirection Direction { get; set; }
	}
}