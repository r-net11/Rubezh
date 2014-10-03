using Microsoft.Practices.Prism.Events;
using FiresecAPI.SKD;
using FiresecAPI.GK;

namespace Infrastructure.Events
{
	public class ShowArchiveEvent : CompositePresentationEvent<ShowArchiveEventArgs>
	{
	}

	public class ShowArchiveEventArgs
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
		public SKDDevice SKDDevice { get; set; }
		public SKDZone SKDZone { get; set; }
		public SKDDoor SKDDoor { get; set; }
	}
}