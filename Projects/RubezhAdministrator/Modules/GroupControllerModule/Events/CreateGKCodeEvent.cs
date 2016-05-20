using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace GKModule.Events
{
	public class CreateGKCodeEvent : CompositePresentationEvent<CreateGKCodeEventArg>
	{
	}

	public class CreateGKCodeEventArg
	{
		public GKCode Code { get; set; }
	}
}