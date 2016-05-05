using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace GKModule.Events
{
	public class CreateGKDelayEvent : CompositePresentationEvent<CreateGKDelayEventArgs>
	{
	}

	public class CreateGKDelayEventArgs
	{
		public bool Cancel { get; set; }
		public GKDelay Delay { get; set; }
	}
}