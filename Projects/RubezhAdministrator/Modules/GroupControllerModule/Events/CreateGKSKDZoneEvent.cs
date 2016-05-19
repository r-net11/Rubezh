using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace GKModule.Events
{
	public class CreateGKSKDZoneEvent : CompositePresentationEvent<CreateGKSKDZoneEventArg>
	{
	}

	public class CreateGKSKDZoneEventArg
	{
		public bool Cancel { get; set; }
		public GKSKDZone Zone { get; set; }
	}
}