using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class ShowGKArchiveEvent : CompositePresentationEvent<ShowGKArchiveEventArgs>
	{
	}

	public class ShowGKArchiveEventArgs
	{
		public GKDevice GKDevice { get; set; }
		public GKZone GKZone { get; set; }
		public GKDirection GKDirection { get; set; }
		public GKPumpStation GKPumpStation { get; set; }
		public GKMPT GKMPT { get; set; }
		public GKDelay GKDelay { get; set; }
		public GKPim GKPim { get; set; }
		public GKGuardZone GKGuardZone { get; set; }
		public GKDoor GKDoor { get; set; }
	}
}