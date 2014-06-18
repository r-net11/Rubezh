using System;
using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;

namespace SKDModule.Events
{
	public class CreateDoorEvent : CompositePresentationEvent<CreateDoorEventArg>
	{
	}

	public class CreateDoorEventArg
	{
		public bool Cancel { get; set; }
		public Guid DoorUID { get; set; }
		public Door Door { get; set; }
	}
}