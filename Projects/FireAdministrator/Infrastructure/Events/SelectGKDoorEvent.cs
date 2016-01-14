using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace Infrastructure.Events
{
	public class SelectGKDoorEvent : CompositePresentationEvent<SelectGKDoorEventArg>
	{
	}

	public class SelectGKDoorEventArg
	{
		public bool Cancel { get; set; }
		public GKDoor Door { get; set; }
	}
}