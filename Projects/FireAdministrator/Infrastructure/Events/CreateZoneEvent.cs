using Microsoft.Practices.Prism.Events;
using System;
using FiresecAPI.Models;

namespace Infrastructure.Events
{
    public class CreateZoneEvent : CompositePresentationEvent<CreateZoneEventArg>
    {
    }

    public class CreateZoneEventArg
    {
        public bool Cancel { get; set; }
        public Zone Zone { get; set; }
    }
}