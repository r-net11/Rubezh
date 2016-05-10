using StrazhAPI.SKD;
using Microsoft.Practices.Prism.Events;

namespace StrazhModule.Events
{
	public class CreateSKDZoneEvent : CompositePresentationEvent<CreateSKDZoneEventArg>
	{
	}

	public class CreateSKDZoneEventArg
	{
		public SKDZone Zone { get; set; }
	}
}