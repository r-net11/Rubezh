using Microsoft.Practices.Prism.Events;
using System;

namespace Infrastructure.Events
{
    public class CreateZoneEvent : CompositePresentationEvent<CreateZoneEventArg>
    {
    }

    public class CreateZoneEventArg
    {
        public bool Cancel { get; set; }
        public Guid ZoneUID { get; set; }
    }
}