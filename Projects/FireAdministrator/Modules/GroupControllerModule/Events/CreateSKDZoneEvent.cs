using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;
using FiresecAPI.GK;

namespace GKModule.Events
{
	public class CreateSKDZoneEvent : CompositePresentationEvent<CreateSKDZoneEventArg>
	{
	}

	public class CreateSKDZoneEventArg
	{
		public GKSKDZone Zone { get; set; }
	}
}