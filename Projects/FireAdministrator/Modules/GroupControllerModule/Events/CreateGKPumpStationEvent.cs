using System;
using RubezhAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateGKPumpStationEvent : CompositePresentationEvent<CreateGKPumpStationEventArgs>
	{
	}

	public class CreateGKPumpStationEventArgs
	{
		public bool Cancel { get; set; }
		public Guid PumpStationUID { get; set; }
		public GKPumpStation PumpStation { get; set; }
	}
}