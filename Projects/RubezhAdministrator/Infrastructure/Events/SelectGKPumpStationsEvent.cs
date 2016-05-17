using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SelectGKPumpStationsEvent : CompositePresentationEvent<SelectGKPumpStationsEventArg>
	{
	}

	public class SelectGKPumpStationsEventArg
	{
		public bool Cancel { get; set; }
		public List<GKPumpStation> PumpStations { get; set; }
	}
}