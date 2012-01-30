using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
    public class CreateZoneEvent : CompositePresentationEvent<CreateZoneEventArg>
    {
    }

    public class CreateZoneEventArg
    {
        public bool Cancel { get; set; }
        public ulong? ZoneNo { get; set; }
    }
}