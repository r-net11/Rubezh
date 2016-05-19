using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace GKModule.Events
{
	public class CreateGKGuardZoneEvent : CompositePresentationEvent<CreateGKGuardZoneEventArg>
	{
	}

	public class CreateGKGuardZoneEventArg
	{
		public bool Cancel { get; set; }
		public GKGuardZone Zone { get; set; }
	}
}