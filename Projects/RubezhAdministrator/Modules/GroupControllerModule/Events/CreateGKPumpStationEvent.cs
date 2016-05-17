using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace GKModule.Events
{
	public class CreateGKPumpStationEvent : CompositePresentationEvent<CreateGKPumpStationEventArgs>
	{
	}

	public class CreateGKPumpStationEventArgs
	{
		public bool Cancel { get; set; }
		public GKPumpStation PumpStation { get; set; }
	}
}