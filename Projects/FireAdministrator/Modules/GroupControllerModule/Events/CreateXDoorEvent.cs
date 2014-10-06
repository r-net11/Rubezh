using System;
using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateXDoorEvent : CompositePresentationEvent<CreateXDoorEventArg>
	{
	}

	public class CreateXDoorEventArg
	{
		public bool Cancel { get; set; }
		public Guid DoorUID { get; set; }
		public GKDoor Door { get; set; }
	}
}