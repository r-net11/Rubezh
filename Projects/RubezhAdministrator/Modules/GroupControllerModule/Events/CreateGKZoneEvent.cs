using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace GKModule.Events
{
	public class CreateGKZoneEvent : CompositePresentationEvent<CreateGKZoneEventArg>
	{
	}

	public class CreateGKZoneEventArg
	{
		public bool Cancel { get; set; }
		public GKZone Zone { get; set; }
	}
}