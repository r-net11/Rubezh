using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;
using FiresecAPI.GK;

namespace GKModule.Events
{
	public class CreateGKSKDZoneEvent : CompositePresentationEvent<CreateGKSKDZoneEventArg>
	{
	}

	public class CreateGKSKDZoneEventArg
	{
		public GKSKDZone Zone { get; set; }
	}
}