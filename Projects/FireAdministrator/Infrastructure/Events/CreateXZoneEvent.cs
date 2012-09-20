using Microsoft.Practices.Prism.Events;
using System;

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