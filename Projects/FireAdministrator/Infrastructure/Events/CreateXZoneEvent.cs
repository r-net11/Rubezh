using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class CreateXZoneEvent : CompositePresentationEvent<CreateXZoneEventArg>
	{
	}

	public class CreateXZoneEventArg
	{
		public bool Cancel { get; set; }
        public Guid ZoneUID { get; set; }
	}
}