using System;
using Microsoft.Practices.Prism.Events;
using XFiresecAPI;

namespace Infrastructure.Events
{
	public class CreateXZoneEvent : CompositePresentationEvent<CreateXZoneEventArg>
	{
	}

	public class CreateXZoneEventArg
	{
		public bool Cancel { get; set; }
        public Guid ZoneUID { get; set; }
		public XZone Zone { get; set; }
	}
}