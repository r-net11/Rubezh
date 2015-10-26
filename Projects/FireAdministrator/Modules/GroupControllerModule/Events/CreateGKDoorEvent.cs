using System;
using RubezhAPI.SKD;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

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