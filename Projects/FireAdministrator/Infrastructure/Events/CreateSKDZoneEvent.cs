using System;
using Microsoft.Practices.Prism.Events;
using XFiresecAPI;
using FiresecAPI;

namespace Infrastructure.Events
{
	public class CreateSKDZoneEvent : CompositePresentationEvent<CreateSKDZoneEventArg>
	{
	}

	public class CreateSKDZoneEventArg
	{
		public bool Cancel { get; set; }
        public Guid ZoneUID { get; set; }
		public SKDZone Zone { get; set; }
	}
}