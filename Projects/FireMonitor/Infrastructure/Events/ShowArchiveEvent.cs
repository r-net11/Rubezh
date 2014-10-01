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
		public XDevice GKDevice { get; set; }
		public XZone GKZone { get; set; }
		public XDirection GKDirection { get; set; }
		public XPumpStation GKPumpStation { get; set; }
		public XMPT GKMPT { get; set; }
		public XDelay GKDelay { get; set; }
		public XPim GKPim { get; set; }
		public XGuardZone GKGuardZone { get; set; }
		public XDoor GKDoor { get; set; }
		public SKDDevice SKDDevice { get; set; }
		public SKDZone SKDZone { get; set; }
		public SKDDoor SKDDoor { get; set; }
	}
}