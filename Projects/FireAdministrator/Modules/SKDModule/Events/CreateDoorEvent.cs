using System;
using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;
using FiresecAPI.SKD;

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