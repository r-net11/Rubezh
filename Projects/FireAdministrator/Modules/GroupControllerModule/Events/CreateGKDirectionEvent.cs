using System;
using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateGKDirectionEvent : CompositePresentationEvent<CreateGKDirectionEventArg>
	{
	}

	public class CreateGKDirectionEventArg
	{
		public bool Cancel { get; set; }
		public Guid DirectionUID { get; set; }
		public GKDirection Direction { get; set; }
	}
}