using System;
using RubezhAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
    public class CreateGKDelayEvent : CompositePresentationEvent<CreateGKDelayEventArgs>
    {
    }

    public class CreateGKDelayEventArgs
    {
        public bool Cancel { get; set; }
        public Guid DelayUID { get; set; }
        public GKDelay Delay { get; set; }
    }
}
