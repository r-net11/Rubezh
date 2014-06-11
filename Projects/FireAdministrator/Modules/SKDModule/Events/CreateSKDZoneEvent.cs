using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;

namespace SKDModule.Events
{
	public class CreateSKDZoneEvent : CompositePresentationEvent<CreateSKDZoneEventArg>
	{
	}

	public class CreateSKDZoneEventArg
	{
		public SKDZone Zone { get; set; }
	}
}