using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace Infrastructure.Events
{
	public class SelectGKPumpStationEvent : CompositePresentationEvent<SelectGKPumpStationEventArg>
	{
	}

	public class SelectGKPumpStationEventArg
	{
		public bool Cancel { get; set; }
		public GKPumpStation PumpStation { get; set; }
	}
}