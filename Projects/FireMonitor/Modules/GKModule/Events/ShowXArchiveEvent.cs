using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class ShowXArchiveEvent : CompositePresentationEvent<ShowXArchiveEventArgs>
	{
	}

	public class ShowXArchiveEventArgs
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
	}
}