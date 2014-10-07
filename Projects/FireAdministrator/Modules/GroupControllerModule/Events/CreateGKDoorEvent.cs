using System;
using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;
using FiresecAPI.GK;

namespace GKModule.Events
{
	public class CreateGKDoorEvent : CompositePresentationEvent<CreateGKDoorEventArg>
	{
	}

	public class CreateGKDoorEventArg
	{
		public GKDoor GKDoor { get; set; }
	}
}