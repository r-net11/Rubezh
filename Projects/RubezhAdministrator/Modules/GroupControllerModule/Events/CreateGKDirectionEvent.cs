using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace GKModule.Events
{
	public class CreateGKDirectionEvent : CompositePresentationEvent<CreateGKDirectionEventArg>
	{
	}

	public class CreateGKDirectionEventArg
	{
		public bool Cancel { get; set; }
		public GKDirection Direction { get; set; }
	}
}